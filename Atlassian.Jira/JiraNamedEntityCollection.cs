﻿using Atlassian.Jira.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Atlassian.Jira
{
    public class JiraNamedEntityCollection<T> : Collection<T>, IRemoteIssueFieldProvider where T : JiraNamedEntity
    {
        protected readonly Jira _jira;
        protected readonly string _projectKey;
        protected readonly string _fieldName;
        private readonly List<T> _originalList;

        internal JiraNamedEntityCollection(string fieldName, Jira jira, string projectKey, IList<T> list)
            : base(list)
        {
            _fieldName = fieldName;
            _jira = jira;
            _projectKey = projectKey;
            _originalList = new List<T>(list);
        }

        public static bool operator ==(JiraNamedEntityCollection<T> list, string value)
        {
            return (object)list == null ? value == null : list.Any(v => v.Name == value);
        }

        public static bool operator !=(JiraNamedEntityCollection<T> list, string value)
        {
            return (object)list == null ? value == null : !list.Any(v => v.Name == value);
        }

        /// <summary>
        /// Removes an entity by name.
        /// </summary>
        /// <param name="name">Entity name.</param>
        public void Remove(string name)
        {
            this.Remove(this.Items.First(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }

        RemoteFieldValue[] IRemoteIssueFieldProvider.GetRemoteFields()
        {
            var fields = new List<RemoteFieldValue>();

            if (_originalList.Count() != Items.Count() || _originalList.Except(Items).Any())
            {
                var field = new RemoteFieldValue()
                {
                    id = _fieldName,
                    values = Items.Select(e => e.Id).ToArray()
                };
                fields.Add(field);
            }

            return fields.ToArray();
        }
    }
}
