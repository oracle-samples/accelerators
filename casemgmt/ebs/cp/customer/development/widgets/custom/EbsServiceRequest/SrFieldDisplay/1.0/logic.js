/* * *******************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  included in the original distribution.
 *  Copyright (c) 2014, 2015, 2016, Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSVC + EBS Enhancement
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 15.8 (August 2015)
 *  EBS release: 12.1.3
 *  reference: 150505-000099, 150420-000127
 *  date: Thu Nov 12 00:52:37 PST 2015

 *  revision: rnw-15-11-fixes-release-1
 *  SHA1: $Id: ac91f86b6329dda57d5d618543ea3c1e7f5d5acb $
 * *********************************************************************************************
 *  File: logic.js
 * ****************************************************************************************** */

RightNow.namespace('Custom.Widgets.EbsServiceRequest');
Custom.Widgets.EbsServiceRequest.SrFieldDisplay = RightNow.Widgets.extend({
    /**
     * Widget constructor.
     */
    constructor: function() {
        this._contentDiv = this.Y.one(this.baseSelector + '_Content');

        // listen to the 'SR Detail Returned' event fired by GetSrDetail
        RightNow.Event.subscribe('evt_SrDetailReturned', this._onSrDetailReturned, this);
    },
    /**
     *  render a single field of the SR detail to the view based on the field name
     */
    _onSrDetailReturned: function(type, args) {
        this.renderView({
            label: this.data.js.label,
            value: args[0].data.sr_detail_data[this.data.js.name]
        });
    },
    /**
     * Renders the `view.ejs` JavaScript template.
     */
    renderView: function(data) {
        var content = new EJS({text: this.getStatic().templates.view}).render({data: data});
        this._contentDiv.set('innerHTML', content);
    }
});