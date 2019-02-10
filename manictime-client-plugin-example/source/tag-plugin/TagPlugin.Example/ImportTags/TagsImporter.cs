﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finkit.ManicTime.Common.TagSources;
using Finkit.ManicTime.Shared.Tags.Labels;
using TimeTrackingService;
using WorkItemServices;

namespace TagPlugin.ImportTags
{
    public class TagsImporter
    {
        private readonly WorkItemClient _workItemClient;

        private readonly TimeTrackingClient _timeTrackingClient;

        public TagsImporter(string organization, string personalAccessToken, string timeTrackingToken)
        {
            _workItemClient = new WorkItemClient(personalAccessToken, organization);
            _timeTrackingClient = new TimeTrackingClient(timeTrackingToken);
        }

        public async Task<List<TagSourceItem>> GetTags()
        {
            var me = await _timeTrackingClient.GetMe();

            var workItemRefsResponse = await _workItemClient
                .GetAssignedWorkItemReferences(me.User.UniqueName);

            var workItems = await _workItemClient
                .GetWorkItemsByReference(workItemRefsResponse.WorkItems);

            var tags = workItems
                .Select(item => new TagSourceItem
                {
                    Tags = new[] { $"#{item.Id}", item.Fields.TeamProject, TagLabels.Billable, ClientPlugin.HiddenTagLabel },
                    Notes = item.Fields.Title
                });

            return tags.ToList();
        }
    }
}