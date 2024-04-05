# GroupFlightPlanner

A C# and ASP.NET web app which displays information about treeplanting events, flights to that event location, and volunteers who are going to the event.

Collaborators and Contributions:
### Sion Lee 
1. Fixed URL string but on Event and Group controllers for Associate and Unassociate methods
2. Created new header menu to allow users to navigate through all the pages efficiently
3. Refactored merged code to fix spelling errors and clear labelling
4. Collaborated with group members to solve issues during migration and merging tasks such as creating new local databases and merging main branch into working branch

### Arnulfo Sanchez

### Nhi Nguyen
Worked with association between Events and Groups.
1. Create a migration and table for many-many relationships between groups and events.
2. Created and updated View Model for DetailsEvent, and DetailsGroups to link associations. Also, Events and Groups have not been unassociated.
   - DetailsEvent.
   - DetailsGroup.
3. Worked with EventController, EventDataController to get data from eventsgroups table to show joined groups on event details, and list unassociated groups on the select box.
   - EventController:
        - Detail(show associated groups with this event).
   - EventDataController.
     - ListEventsForGroup.
     - AssociateEventWithGroup.
     - UnAssociateEventWithGroup.
4. Worked with GroupDataController, GroupDataController to get data from eventsgroups table to show registered events on group details, and list unassociated events on the       select box.
   - GroupController:
        - Detail(show all events were joined in by this group).
   - GroupDataController:
       - ListGroupsForEvent.
       - ListGroupsNotJoinInEvent
5. Showed associations of events and groups, and events and groups have not been unassociated on the view of event details and group details.

