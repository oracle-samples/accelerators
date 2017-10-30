<?php

/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set 
 *  published by Oracle under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: Telephony and SMS Accelerator for Twilio
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17D (November 2017)
 *  reference: 161212-000059
 *  date: Monday Oct 30 13:4:53 UTC 2017
 *  revision: rnw-17-11-fixes-releases
 * 
 *  SHA1: $Id: bd689d5ed76d1da7ab738d8697e2f3e66d7f16aa $
 * *********************************************************************************************
 *  File: IVRConfig.php
 * ****************************************************************************************** */

use RightNow\Connect\v1_3 as Connect;

//require_once __DIR__ . "/CTIConfig.php";

class IVRConfig
{
    static $config;
    public static function get( $key )
    {
        if( array_key_exists( $key, self::$config) )
        {
            $data = self::$config[$key];
            if( is_callable( $data ) )
            {
                $data = $data();
            }
            return $data;
        }
        return false;
    }
}

IVRConfig::$config = [
    "voice" => function () {
        return \CTIConfig::get("voice", "woman");
    },
    
    "flow" => [
        0 => [
            "dialogs" => [
                /* These dialogs are coming from MessageBase. To change the dialog, 
                   please edit the MessageBase. The following is a MessageBase
                   key corresponding to a dialog text.
                */
                CUSTOM_MSG_CTI_Flow_0_Dialogs
            ],
            "redirect" => 2
        ],

        1 => [
            "enqueue" => [
                "dialogs" => [
                    CUSTOM_MSG_CTI_Flow_1_Dialogs
                ]
            ]
        ],

        2 => [
            "preProcess" => function( $connect )
            {
                $config = [
                    "repeat" => CUSTOM_MSG_CTI_Flow_2_Repeat,
                    "digits" => 1,
                    "next"   => [
                        1 => 3, 
                        2 => 4,
                        3 => 5,
                        0 => 1
                    ]
                ];
                $data = [];

                $incident = $connect->findOpenIncident( $_POST["From"] );
                if( is_array( $incident ) )
                {
                    $config["dialogs"] = [
                        CUSTOM_MSG_CTI_Flow_2_Dialogs_OpenIncident_P1,
                        $incident["ref"],
                        CUSTOM_MSG_CTI_Flow_2_Dialogs_OpenIncident_P2
                    ];

                    $config["next"] = [
                        1 => 1, 
                        2 => 4,
                        3 => 5,
                        0 => 1
                    ];

                    $data["incident"] = $incident["id"];
                }
                else if( $incident >0 )
                {
                    $config["dialogs"] = [
                        CUSTOM_MSG_CTI_Flow_2_Dialogs_OpenIncidents
                    ];
                }
                else
                {
                    $config["dialogs"] = [
                        CUSTOM_MSG_CTI_Flow_2_Dialogs_NoIncidents
                    ];
                    $config["next"] = [
                        1 => 4, 
                        2 => 5,
                        0 => 1
                    ];
                }

                return [
                    "config" => [
                        "gather" => $config
                    ],
                    "data"   => $data
                ];

            },
            
            "errorDialogs" => [
                CUSTOM_MSG_CTI_Flow_2_ErrorDialogs
            ],
            "redirect" => 2
        ],

        3 => [
            "gather" => [
                "dialogs" => [
                    CUSTOM_MSG_CTI_Flow_3_Dialogs
                ],
                "repeat" => CUSTOM_MSG_CTI_Flow_3_Repeat,
                "digits" => 12,
                "timeout"=> 15,
                "next" => 1
            ],
            "process" => function ( $param, $data )
            {
                $refnum = $data['Digits'];
                if( strlen( $refnum ) < 12 )
                    return false;
                
                $refnum = substr( $refnum, 0, 6) . "-" . substr( $refnum, 6, 6);
                $incident = false;
                try
                {
                    $incident = Connect\Incident::fetch($refnum);
                }
                catch( Exception $e )
                {
                    return false;
                }
                
                return [ 
                    "incident" => $incident->ID
                ];

            },
            "errorDialogs" => [
                CUSTOM_MSG_CTI_Flow_3_ErrorDialogs
            ],
            "redirect" => 3
        ],

        4 => [
            "gather" => [
                "dialogs" => [
                    CUSTOM_MSG_CTI_Flow_4_Dialogs
                ],
                "repeat" => CUSTOM_MSG_CTI_Flow_4_Repeat,
                "timeout" => 10,
                "next" => 1
            ],
            "process" => function ( $param, $data )
            {
                $CI = get_instance();

                $CI->load->model('custom/CTIConnect');

                $contact_data = $CI->CTIConnect->getContactData( $data["From"] );
                $incident = $CI->CTIConnect->createIncidentWithProductId( $contact_data["id"], $_POST["Digits"] );

                if( false === $incident )
                {
                    return false;
                }
                return [
                    "incident" => $incident
                ];
            },
            "errorDialogs" => [
                CUSTOM_MSG_CTI_Flow_4_ErrorDialogs
            ],
        ],

        5 => [
            "dialogs" => [
                CUSTOM_MSG_CTI_Flow_5_Dialogs
            ],
            "record" => [
                "beep" => true, 
                "transcribe" => true,
                "timeout" => 120,
                "next" => 6
            ],
            "redirect" => 6
        ],

        6 => [
            "dialogs" => [
                CUSTOM_MSG_CTI_Flow_6_Dialogs
            ]
        ]
    ]
];
