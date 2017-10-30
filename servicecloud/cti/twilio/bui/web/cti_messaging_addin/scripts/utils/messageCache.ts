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
 *  SHA1: $Id: 1da0f95da651d91031c287ce3e0d368a7e8e7868 $
 * *********************************************************************************************
 *  File: messageCache.ts
 * ****************************************************************************************** */

import {Message} from "../model/message";
/**
 * This class keeps all messages for
 * currently opened workspaces
 */
export class MessageCache {
    private static messages: {[key: string]: Message} = {};
    private static cacheSize: number = 0;

    /**
     * Returns the message with given key
     *
     * @param key
     * @returns {Message}
     */
    public static get(key: string): Message {
        return MessageCache.messages[key];
    }

    /**
     * Add a message to the cache, with given key
     *
     * @param key
     * @param message
     */
    public static put(key: string, message: Message): void {
        MessageCache.messages[key] = message;
        MessageCache.cacheSize++;
    }

    /**
     * Removes  message with given key
     *
     * @param key
     */
    public static remove(key: string): void {
        if(MessageCache.messages[key]){
            MessageCache.messages[key] = null;
            MessageCache.cacheSize--;
        }
    }

    public static clearMessage(key: string): void {
        if(MessageCache.messages[key]){
            MessageCache.messages[key].message = '';
        }
    }

    /**
     * Clear all messages from cache
     *
     */
    public static clearCache(): void {
        MessageCache.messages = {};
        MessageCache.cacheSize = 0;
    }

    /**
     * Return the number of messages in cache
     * 
     * @returns {number}
     */
    public static getCacheSize(): number {
        return MessageCache.cacheSize;
    }
}