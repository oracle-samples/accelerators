/* * *******************************************************************************************
 *  This file is part of the Oracle B2C Service Accelerator Reference Integration set published
 *  by Oracle B2C Service licensed under the Universal Permissive License (UPL), Version 1.0 as shown at 
 *  http://oss.oracle.com/licenses/upl
 *  Copyright (c) 2023, Oracle and/or its affiliates.
 ***********************************************************************************************
 *  Accelerator Package: Incident Text Based Classification
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 23C (August 2023) 
 *  date: Tue Aug 22 11:57:47 IST 2023
 
 *  revision: RNW-23C
 *  SHA1: $Id: 5f97aaff4f2b6d85939312661c8a01db16ad6bd4 $
 * *********************************************************************************************
 *  File: engagementPanelJs.d.ts
 * ****************************************************************************************** */
declare module IOracleChatClient {
  var IOracleChatClient: any;

  export interface IEngagement {
    MyRole: any;
    EngagementId: string;
    IncidentId: string;
    QueueId: string;
    QueueName: string;
    Concluded: boolean;
    Accepted: boolean;
    PostPermission: boolean;
    getEngagementId: () => Promise<string>;
    getIncidentId: () => Promise<string>;
    getQueueId: () => Promise<string>;
    messagePosted: (currentMessage: ORACLE_SERVICE_CLOUD.IChatMessage) => void;
    getMessages: () => Promise<IChatMessage[]>;
    concluded: (chatConcluded: any) => boolean;
  }
  export interface IEngagement {}
  export interface IChatEngagementRemovedEventArgs {}
  export interface IChatEngagementAssignmentEventArgs {}
  export interface IChatMessage {
    Sender: any;
    Body: any;
    IsRichText: any;
    IsOffRecord: any;
    Visibility: any;
    PostTime: Date;
  }
  export interface IAgentSession {
    IsLoggedIn: boolean;
    getAgentSession: () => Promise<IAgentSession>;
    getEngagement: (engagementId: number) => Promise<IEngagement>;
    getCurrentEngagement: () => Promise<IEngagement>;
    canPullEngagement: () => Promise<boolean>;
    sessionStatusChanged: (IChatSessionStatusEventArgs) => Promise<IChatSessionStatusEventArgs>;
    engagementRemoved: (IChatEngagementRemovedEventArgs) => None;
    engagementAssigned: (IChatEngagementAssignmentEventArgs) => None;
    engagementAccepted: (IChatEngagementAcceptedEventArgs) => None;
    
  }
  
  export function getAgentSession(): Promise<IAgentSession>;
}
