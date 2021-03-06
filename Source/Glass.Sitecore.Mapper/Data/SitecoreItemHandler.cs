﻿/*
   Copyright 2011 Michael Edwards
 
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Sitecore.Mapper.Configuration.Attributes;
using Sitecore.Data;
using Glass.Sitecore.Mapper.Configuration;

namespace Glass.Sitecore.Mapper.Data
{
    public class SitecoreItemHandler : AbstractSitecoreDataHandler
    {

        public bool IsLazy
        {
            get;
            set;
        }

        public string Path { get; set; }
        public Guid Id { get; set; }
        public SitecoreProperty Property { get; set; }

        public override bool WillHandle(Configuration.SitecoreProperty property, IEnumerable<AbstractSitecoreDataHandler> datas, Dictionary<Type, Configuration.SitecoreClassConfig> classes)
        {
            return property.Attribute is SitecoreItemAttribute && classes.ContainsKey(property.Property.PropertyType);
            
        }

        public override object GetValue(global::Sitecore.Data.Items.Item item, ISitecoreService service)
        {
            if (Id != Guid.Empty)
            {
                var scItem = service.Database.GetItem(new ID(Id));
                return service.CreateClass(IsLazy, false, Property.Property.PropertyType, scItem);
            }
            else if (!Path.IsNullOrEmpty())
            {
                var scItem = service.Database.GetItem(Path);
                return service.CreateClass(IsLazy, false, Property.Property.PropertyType, scItem);
            }
            else
            {
                return null;
            }
        }

        public override void SetValue(global::Sitecore.Data.Items.Item item, object value, ISitecoreService service)
        {
            throw new MapperException("SitecoreItem mappings can not be set");
        }

        public override bool CanSetValue
        {
            get { return false ; }
        }
        public override void ConfigureDataHandler(Configuration.SitecoreProperty scProperty)
        {
            var attr = scProperty.Attribute as SitecoreItemAttribute;

            if (attr.Id.IsNullOrEmpty())
            {
                Path = attr.Path;
            }
            else
            {
                Guid id = Guid.Empty;
                if (attr.Id.GuidTryParse(out id))
                {
                    Id = id;
                }
                else
                    throw new MapperException("Id is not a Guid on SitecoreItemAttribute\n\rClass: {0}\n\rMember:{1}".Formatted(scProperty.Property.ReflectedType.FullName, scProperty.Property.Name));

            }

            this.IsLazy = attr.IsLazy;
            this.Property = scProperty;
            base.ConfigureDataHandler(scProperty);
        }

    }
}
