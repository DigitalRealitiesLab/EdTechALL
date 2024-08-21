#Q&A 
##This Documentation
This documentation is generated automatically every time there is a commit on the master branch. You can configure the build pipeline in the gitlab-ci.yml file in the repository. 
The files to create the documentation can be found in the Documentation folder of the repository. Under manual you can add new pages by creating .md files and adding them to the toc.yml file
The documentation website is hosted and automatically updated with every commit by gitlab pages here - [http://fhs38598.pages.mediacube.at/edtechall](http://fhs38598.pages.mediacube.at/edtechall). Triple slash comments in code `/// ` become a part of the API documentation.
The documentation website is publicly accessable. Brigitte has tried to activate privacy options for gitlab pages before, but that led to errors in the gitlab config --> Pages stays public in the near future.

##Does or will the EdTechALL App use Internet?
We currently do not support functionality that requires internet - simply because we can not expect all schools participating in tests to have stable internet access for 25 devices at once. 

##Can I use a local Network instead?
Yes you may use a loacal Network. [Here](https://github.com/Unity-Technologies/arfoundation-samples/tree/main/Assets/Scripts/Multipeer) is a sample repository from Unity that is working for up to 8 local users. You just need to add it to the EdTechALL project. 

##How is h5p content created and how is it displayed?
The h5p content in the app is created by our partners at PH Salzburg and PLUS (Speciffically Marie & David) with the [Lumi Editor](https://lumi.education/). Your job is to insert the content they create into the app in Unity. Usually they will upload new content to a shared teams folder. In the project files, the hp5 content is stored in Assets > StreamingAssets > h5p. You can freely modify the folder structure below StreamingAssets as long as the paths in the CardData scriptable objects still point to the right files. 
H5p content is exported as a single HTML file - which allows us to display its content inside a browser window in Unity with the plugin [3D Web View by Vuplex](https://developer.vuplex.com/webview/overview). The plugin is well documented on their website. 

##Inspiration for Lesson Media from existing Books
[https://www.oebv.at/corona-vs-digitale-schulbuecher](https://www.oebv.at/corona-vs-digitale-schulbuecher)