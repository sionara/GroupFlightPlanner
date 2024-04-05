# GroupFlightPlanner

A C# and ASP.NET web app which displays information about treeplanting events, flights to that event location, and volunteers who are going to the event.

### Collaboration Together (Sion, Arnulfo, Nhi)
- Setting up the MVP of the 3 Passion Projects in just one project

### Collaborators and Contributions:
### Sion Lee 
Worked with association between Events and Groups, Locations and Flights.
1. Fixed URL string but on Event and Group controllers for Associate and Unassociate methods
   - EventController
   - AssociateEventWithGroup
   - UnAssociateEventWithGroup
2. Created new header menu to allow users to navigate through all the pages efficiently, display correct information on View
   - Views/Shared/_Layout
   - Views/Home/Index
   - Views/Group/Details
   - Views/Event/Details
   - Views/Event/Edit
   - Views/Event/New
3. Refactored merged code to fix spelling errors and clear labelling
4. Collaborated with group members to solve issues during migration and merging tasks such as creating new local databases and merging main branch into working branch
5. Collaborated with problem-solving of integration of the code as well as the elaboration of related methods

### Arnulfo Sanchez
Worked with association between Locations and Flights.
1. **Created relationship (M-M) and migration of locations-flights**
2. **Implemented related methods in DataController and Controller:**
    - ListLocationsForFlight
    - ListFlightsForLocation
    - ListFlightsNotAssociatedForLocation
    - AssociateLocationWithFlight
    - UnAssociateLocationWithFlight 
3. **Implemented methods for Location Controller**
    - Associate (Location - Flight)
    - UnAssociate(Location - Flight)
4. **Updated ViewmModels:**
    - DetailsFlight
    - DetailsLocation
5. **Updated Views:**
    - Flight/Details
    - Location/Details
6. **Created Authorization in Airline, Airplane, and Flight entities**

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

