Licenses and Attribution
At Assets/LICENSE.txt the licenses for all files created by Fachhochschule Salzburg GmbH and their associates can be found.
Other files are redistributed under their original license.
At Assests/ATTRIBUTION.txt the attributions for files under Attribution licenses can be found.
The file Assets/Development/MiniGames/Games/DairyTour/Videos/FillingPlantVideo.mp4 was provided by SalzburgMilch GmbH and the use in a derivative or different work must be requested from them.

Store assets
The following Unity Asset Store model assets need to be bought, downloaded and put into the folder Assets/AssetStore/ModelAssets before opening the Project in Unity Engine:
https://assetstore.unity.com/packages/3d/props/exterior/gas-tank-pbr-116147
https://assetstore.unity.com/packages/3d/props/exterior/hay-bale-square-and-round-3d-model-240239
https://assetstore.unity.com/packages/3d/environments/industrial/polygon-farm-low-poly-3d-art-by-synty-146192
https://assetstore.unity.com/packages/3d/environments/simplepoly-world-low-poly-assets-73353
https://assetstore.unity.com/packages/3d/characters/animals/mammals/domestic-animals-pack-2-75059
The following Unity Asset Store plugin assets need to be bought, downloaded and put into the folder Assets/Plugins before opening the Project in Unity Engine:
https://assetstore.unity.com/packages/tools/integration/upano-126396 (Version 3.0.2)

Map
If you want to use a different map, you need to replace the corresponding images referenced in FloorMapImageLibrary.asset and HandMapImageLibrary.asset.
Leaving images in the libraries empty will result in errors.
The correct measurements of the map parts in metres also need to be entered here.
Since the floor map is separated into a 3x3 grid, the separate parts measurements will be the total map length and with divided by three.
Since the hand map is separated into a 2x2 grid, the separate parts measurements will be the total map length and with divided by two.
The substitute map in the project is expected to be printed on a piece of A3 paper, assuming that with a small border the maps total width will be about 29cm.
A lot of the minigames (prefabs with name ending in "MiniGame.prefab") are scaled and positioned to fit the substitute map and will need to be adjusted to fit a different map.

Apple Developer How To
To build for iOS you will need an Apple Developer Account with the necessary Development Certificates and a development system with XCode (e.g. a Macbook).
First you need to enter your Apple Developer Team ID at Unity Player Settings -> Signing Team ID and make sure that all prerequisites are fulfilled and the build target is set to iOS.
After building you will get an XCode Project and you can open the .xcodeproj file in XCode.
If you want to distribute the build to Testflight or the AppStore, you need to have a Distribution Certificate and set up the corresponding backend on App Store Connect.
You can also directly build the project onto an Apple device connected via USB-C.