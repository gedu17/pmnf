#VidsNET

##2.0.0
###Back-End
- [x] Fix not checking for deleted and viewed flags in virtual view 
- [x] Add Template controller to return templates for front-side modal actions
- [x] Fix new virtual folder creation not creating real item
- [x] Add VirtualFolder value to Item enum 
- [x] Add ability to unset viewed and deleted flags
- [x] Add enum for viewtypes
- [x] Add ModelState validation check in ItemController

###Front-End
- [x] Add actions for items 
- [x] Fix padding for actions button
- [x] Add create folder action
- [x] Fix child items not being removed when item is viewed or deleted
- [x] Add different lists for default, viewed and deleted
- [x] Add separate popover actions for different lists
- [x] Refactor scripts.js to multiple files

##1.9.0
###Back-End
- [x] Trim front-end input
- [x] Fix naming
- [x] Add exception handlers
- [x] Change IUserRepository interface to abstract class
- [x] Fix returning incorrect subtitle if more than one subtitle file found in the directory
- [x] Add support for cross-platform home directory scan
- [x] Fix foreign key support for other dbms

###Front-End
- [x] Reset user forms in settings after submit
- [x] Clean notifications from tabs when switching tabs
- [x] Disable links on folders that do not have children
- [x] Implement Physical view

##1.8.0
###Back-End
- [x] Convert username to lowercase and trim whitespaces when entered
- [x] Add different database contexts depending on wether its Sqlite or any other dbms
- [x] Add IsSqlite boolean constant
- [x] Rename Seen to Viewed in VirtualItems table
- [x] Implement password hashing
- [x] Add Timestamp field in SystemMessages table
- [x] Add AddSystemMessage and SetSystemMessageAsRead to UserData class
- [x] Add SessionHash to User model
- [x] Add SessionHash to view urls
- [x] Check wether SessionHash matches and if user has requested video in his list
- [x] Add UserPath rescan when user paths are updated
- [x] Fix return url not working when logging in

##1.7.0
###Back-End
- [x] Implement user level and active changer
- [x] Fix bug in BaseScanner Sort method
- [x] Fix bug in Scanner GetChildren method
- [x] Add orphin removal in Scanner

###Front-End
- [x] Add Manage users tab in settings
- [x] Add Create user tab in settings
- [x] Implement User paths tab in settings
- [x] Move all tabs in settings to separate partial views

##1.6.0
###Back-End
- [x] Rework BaseScanner from abstract class to normal class
- [x] Rework BaseScanner to only populate items
- [x] Remove VideoScanner and SubtitleScanner
- [x] Add IScannerCondition interface
- [x] Implement IScannerCondition interfaces for videos and subtitles
- [x] Add Item adding to database in Scanner class
- [x] Add option in BaseScanner to ignore hidden files (in linux environment)
- [x] Add sort method to BaseScanner to sort all items in list
- [x] Add HtmlHelpers class with methods to be used in front-end

###Front-End
- [x] Add user paths tree to settings
- [x] Add Virtual items template
- [x] Add Settings template
- [x] Add Login template

##1.5.0
###Back-End
- [x] Add Video and Subtitle serving in ItemController
- [x] Add UserData class to dependency injection
- [x] Move ViewModels to separate directory and namespace
- [x] Add VideoViewer class to serve video and subtitles
- [x] Add VideoViewResult class to return information to controller
- [x] Add MimeTypes to VideoType and SubtitleType enums
- [x] Add CurrentUrl variable to UserData
- [x] Add Special route mapping for video view

##1.4.0
###Back-End
- [x] Add ability to change settings
- [x] Add ability to change password
- [x] Add change password to user repository class
- [x] Add UserData class
- [x] Add ViewModels for data coming from front-end
- [x] Add BaseController from which all other controllers inherit
- [x] Change how claims are structured
- [x] Add Scanner class which uses is used by controller
- [x] Add locks to asynchronous actions in BaseScanner

###Front-End
- [x] Add Virtual view prototype
- [x] Add Scan view prototype
- [x] Add javacript functions to update settings and change password

##1.3.0
###Back-End
- [x] Add Authentication with password
- [x] Add MenuItems and MenuItem class
- [x] Add BaseViewModel class
- [x] Add HomeViewModel, LoginViewModel classes
- [x] Add appropriate namespaces

###Front-End
- [x] Create page layout
- [x] Add authentication with password
- [x] Fix menu bar to reflect on current page

##1.2.0
###Back-End
- [x] Add Dependency injection support for DatabaseContext
- [x] Fix mistype "DatabseContext" to "DatabaseContext"
- [x] Implement methods in ItemController (except view)
- [x] Finalize BaseScanner class
- [x] Implement VideoScanner class
- [x] Implement SubtitleScanner class
- [x] Implement VideoType enum
- [x] Implement SubtitleType enum
- [x] Add FrontEndItem class to bind data from front-end

###Misc
- [x] Change versioning from 0.x.y to x.y.z

##1.1.0
###Back-End
- [x] Add BaseScanner class
- [x] Add VideoScanner class
- [x] Add ItemType to Item class
- [x] Add static file serving

###Front-End
- [x] Add Bootstrap 
- [x] Add jQuery


##1.0.0
###Back-End
- [x] Add EntityFramework support
- [x] Add Session support
- [x] Add MVC support
- [x] Add Logging support
- [x] Create entity models 
- [x] Create item type enum

###Front-End
- [x] Add basic index template