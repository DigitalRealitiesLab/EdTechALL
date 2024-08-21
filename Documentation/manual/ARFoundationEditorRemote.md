#ARFoundation Editor Remote
Unity has no integration for testing apps built with ARFoundation in Play Mode. Every time you want to test some changes, it would require a time consuming new build. 
[AR Foundation Editor Remote](https://assetstore.unity.com/packages/tools/utilities/ar-foundation-editor-remote-168773) is a plugin from the Unity Asset Store that allows to view your app on a device connected via USB or Wifi.
We have 3 seats for the plugin and there is a newer version available [AR Foundation Editor Remote 2](https://assetstore.unity.com/packages/tools/utilities/ar-foundation-remote-2-0-201106) that we don't have seats for yet. 

##Setup
There is an extensive setup manual that comes with the plugin. It can be found at Assets > Plugins > ARFoundationEditorRemoteInstaller > Documentation.md

##Troubleshooting
* Tracking is super slow: Try adding your Reference Image Libraries to the settings in Assets > Plugins > ARFoundationEditorRemoteInstaller > Resources > Settings.asset and dont forget to **rebuild the ARCompanion App**.
* The framerate is super low: Try connecting via USB cable or use another Wifi Network eg. the MMTPrototyping instead of eduroam. There were problems with using eduroam a while back but the issue seems to have resolved itself. 
* There is no connection: 
	* The IP address the companion app uses to communication with your Mac can change irregularly. Go to settings in Assets > Plugins > ARFoundationEditorRemoteInstaller > Resources > Settings.asset and check if it is the same as in the companion app.
	* Check if AR Foundation Remote is activated in the Project Settings under XRPlug-in Management - Standalone - Plug-in Providers
* Unity crashes: Sadly I don't have a solution for this - AR Foundation Editor Remote is somewhat unstable and causes frequent crashes especially in debug mode. It could be caused by some incompatibility issue with M1 processor based Macs. 