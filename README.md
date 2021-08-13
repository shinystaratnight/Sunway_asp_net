# Sunway Web Code

This is the collection of web applications and the dependencies required to run them.

## Sunway Booking Site
The web.booking site contains the code for sunway.ie/booking  
In order to run the site locally, you will need to run the Web.Template project as well as the Web.Booking project.

### Running Web.Template & Web.Booking
In order to run the web.template project you will need to have node installed, it can be downloaded from https://nodejs.org/download/release/v10.14.0/  
The version of node used to create the web.template and web.booking projects was 10.14.0.  
Once node is installed, navigate to the web.template folder containing the package.json file using command prompt/powershell and type the following.
```
npm install
npm run vendor
npm run sunway
```
Do the same for the Web.Booking project, you should see a lot of linting text checking the validity of the javascript being compiled.  
Once the javascript has been compiled you will be able to run each project.  
Web.Booking uses styling from Web.Template so it's best to make sure Web.Template is running before Web.Booking.

The above example is for running the sunway trade version of the site.  
In order to run the B2C version of the site replace
```
npm run sunway
```

with
```
npm run sunwayb2c
```

```
When initially running the web.booking application, it may throw an error in PageController.cs, you can continue from this and remove the trailing / from the url in the browser to reload the page.
```

## Choosing which Environment to run against

The SiteURL in the web.config of the Web.Booking, Web.Template and Web.TradeMMB applications determines which environment the application will run against, the url must match one of the urls in the WebsiteURLSettings.json file in each project.


## Running Trade MMB

The Trade MMB application requires the user to be logged in in order to work.  
Run the Web.Template project first using the instructions above, then run the Web.TradeMMB application.

You will need to run the npm process detailed above in the Web.TradeMMB folder with the package.json file.

## Running Web.MMB
The Web.MMB application was built on older technology and doesn't use React, this means that to run it you don't need to run the same commands as above, however it does use scss files for styling which need to be compiled.  
The tool used for this is gulp, to install it, ensure that node is installed and run the following command in command prompt/powershell in the Web.MMB folder that contains the gulpfile.js file.
```
npm install
```
Once everything is installed, run gulp with the following command in the folder containing the gulpfile.js.
```
gulp
```
Once this is done, you should be able to run the application.

## Encryption Key
In order to run the applications correctly, an EncryptionKey needs to be declared in the web.config of the application. This has been declared in the provided web.configs, it can be changed, however it's important that each site use the same key as it's used to encrypt/decrypt cookie data which is shared across the Booking, Template and TradeMMB sites.

## Node version issues
You may encounter issues running the node instructions if you have a different version of node installed.  
If this happens you can download a utility tool called nvm, which allows you to switch which node version is installed and currently being used.  
It can be downloaded from here https://github.com/coreybutler/nvm-windows/releases and instructions for how to change versions can be found here https://github.com/coreybutler/nvm-windows

