##Create a custom default raster renderer

###Purpose  
This sample shows how to create a custom default raster renderer. This sample creates a dynamic-link library (DLL), which is registered to the component category, and changes the default behavior to add a 1-bit tagged image file format (TIFF) image.  


###Usage
1. Start Visual Studio, open the solution file, and compile the program. A DLL is generated and registered in the ESRI RasterRendererMakers component category.  
1. Run category.exe from the ArcGIS bin directory to confirm that the custom DLL was added to that category.  
1. Start ArcMap and add a 1-bit TIFF image. The image displays as a unique value renderer with red color.  
1. To return to the ArcGIS built-in renderer for the 1-bit TIFF, use the esriregasm.exe utility from the Program Files\Common Files\ArcGIS\bin folder to unregister the DLL.   









---------------------------------

####Licensing  
| Development licensing | Deployment licensing | 
| :------------- | :------------- | 
| ArcGIS for Desktop Basic | ArcGIS for Desktop Basic |  
| ArcGIS for Desktop Standard | ArcGIS for Desktop Standard |  
| ArcGIS for Desktop Advanced | ArcGIS for Desktop Advanced |  
| Engine Developer Kit |  |  


