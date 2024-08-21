#Apple
Our target platform is iOS there are several things unique to building for Apple products.

##XCode
Unity can't build directly for your iOS devices, as you might be used to from working wiht Android. Instead it builds an XCode project for you that you have to 

##Appstore Connect & Testflight
Appstore Connect is the developers connection to bringing apps to the appstore. Testflight is the beta channel of the appstore that allows to restrict access to a restricted audience - perfect for us! 

##Uploading a Build
1. Make a build with Unity
2. Open the resulting Project in XCode
3. Optionally do some testing here
4. Increment the build number, and optionally change the version number. Changing the version number causes the build to go through a lengthy (2-3 day) verification process by apple, while incementing the build number does not cause this issue. 
5. Check if the Unity Framework is linked
6. Check if the Application Supports Itunes Filesharing flag is set to TRUE in the Info.plist file (needed to gather saved files after using the app)
7. Go to Project > Archive and wait for it to finish
8. Validate the ipa (if you are sure that your configuration is correct you can skip this step and go right to Distribute)
9. Distribute the ipa (Let XCode manage everything automatically if possible)

###Troubleshooting
* I can't upload a build because I don't have the right signing certificate: Check if you have access to coloud managed signing certificates and ask Hilmar to give you that permission if you don't have it. Otherwise google - other developers surely had the same problem before. It can usually be fixed by creating your own certificates in appstore connect and having the right permissions, that Apple somehow continues to change. 
*