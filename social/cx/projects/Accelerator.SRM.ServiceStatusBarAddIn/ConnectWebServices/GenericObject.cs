/* *********************************************************************************************
 *  This file is part of the Oracle Service Cloud Accelerator Reference Integration set published
 *  by Oracle Service Cloud under the Universal Permissive License (UPL), Version 1.0
 *  Copyright (c) 2014, 2015, 2016, 2017 Oracle and/or its affiliates. All rights reserved.
 ***********************************************************************************************
 *  Accelerator Package: OSvC + SRM Integration Accelerator
 *  link: http://www.oracle.com/technetwork/indexes/samplecode/accelerator-osvc-2525361.html
 *  OSvC release: 17.2 (February 2017) 
 *  reference: 160628-000117
 *  date: Fri Feb 10 19:47:42 PST 2017
 
 *  revision: rnw-17-2-fixes-release-2
 *  SHA1: $Id: f994c7b4e0a51fee4f5aff6cc1ff77775c7e0094 $
 * *********************************************************************************************
 *  File: GenericObject.cs
 * *********************************************************************************************/

using System;
using Accelerator.SRM.SharedServices.RightNowServiceReference;

namespace Accelerator.SRM.SharedServices
{
    /// <summary>
    /// Factory class for constructing generic objects in ConnectWebServices
    /// </summary>
    public static class GenericObjectFactory
    {
        /// <summary>
        /// Create a generic text field
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static GenericField CreateGenericTextField(string name, string value)
        {
            GenericField f = new GenericField();
            f.dataType = DataTypeEnum.STRING;
            f.dataTypeSpecified = true;
            f.name = name;
            f.DataValue = new DataValue();
            f.DataValue.Items = new object[1];
            f.DataValue.Items[0] = value;
            f.DataValue.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.StringValue };

            return f;
        }

        /// <summary>
        /// Create a generic int field
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static GenericField CreateGenericIntegerField(string name, int value)
        {
            GenericField f = new GenericField();
            f.dataType = DataTypeEnum.INTEGER;
            f.dataTypeSpecified = true;
            f.name = name;
            f.DataValue = new DataValue();
            f.DataValue.Items = new object[1];
            f.DataValue.Items[0] = value;
            f.DataValue.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.IntegerValue };

            return f;
        }

        /// <summary>
        /// Create a generic bool field
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static GenericField CreateGenericBoolField(string name, bool value)
        {
            GenericField f = new GenericField();
            f.dataType = DataTypeEnum.BOOLEAN;
            f.dataTypeSpecified = true;
            f.name = name;
            f.DataValue = new DataValue();
            f.DataValue.Items = new object[1];
            f.DataValue.Items[0] = value;
            f.DataValue.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.BooleanValue };

            return f;
        }

        /// <summary>
        /// Create a generic date/time field
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static GenericField CreateGenericDateTimeField(string name, DateTime value)
        {
            GenericField f = new GenericField();
            f.dataType = DataTypeEnum.DATETIME;
            f.dataTypeSpecified = true;
            f.name = name;
            f.DataValue = new DataValue();
            f.DataValue.Items = new object[1];
            f.DataValue.Items[0] = value;
            f.DataValue.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.DateTimeValue };

            return f;
        }

        /// <summary>
        /// Create a generic namedID field
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static GenericField CreateGenericNamedIDField(string name, NamedID value)
        {
            GenericField f = new GenericField();
            f.dataType = DataTypeEnum.NAMED_ID;
            f.dataTypeSpecified = true;
            f.name = name;
            f.DataValue = new DataValue();
            f.DataValue.Items = new object[1];
            f.DataValue.Items[0] = value;
            f.DataValue.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.NamedIDValue };

            return f;
        }

        /// <summary>
        /// Create a generic object field.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static GenericField CreateGenericObjectField(string name, GenericObject value)
        {
            GenericField f = new GenericField();
            f.dataType = DataTypeEnum.OBJECT;
            f.dataTypeSpecified = true;
            f.name = name;
            f.DataValue = new DataValue();
            f.DataValue.Items = new object[1];
            f.DataValue.Items[0] = value;
            f.DataValue.ItemsElementName = new ItemsChoiceType[] { ItemsChoiceType.ObjectValue };

            return f;
        }

        /// <summary>
        /// Create a generic object
        /// </summary>
        /// <param name="objName"></param>
        /// <param name="objNamespace"></param>
        /// <param name="objId"></param>
        /// <returns></returns>
        public static GenericObject CreateGenericObject(string objName, string objNamespace, int? objId = null)
        {
            GenericObject go = new GenericObject();
            go.ObjectType = new RNObjectType();
            go.ObjectType.Namespace = objNamespace;
            go.ObjectType.TypeName = objName;

            if (objId != null)
            {
                go.ID = new ID();
                go.ID.id = (int)objId;
                go.ID.idSpecified = true;
            }

            return go;
        }
    }
}
