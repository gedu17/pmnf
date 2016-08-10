#VidsNET

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