using System;
using System.Collections;
using System.Text;
using System.Drawing;

namespace BitmapToHTML{
	/// <summary>
	/// Manages creating one CSS class for each colour in a bitmap 
	/// </summary>
	public class CSSClassManager{
	  // we'll use a hashtable so that checking for an existing class 
	  // will be very quick
	  Hashtable hashClasses; 
	
		public CSSClassManager(){
		  hashClasses = new Hashtable();
		}
		
		/// <summary>
		///   Take the colour components of a pixel and return
		///   either a pre-existing CSS class name for that colour
		///   or create a class for that colour and return it's name
		/// </summary>
		/// <param name="bRed">Red component of a pixel</param>
		/// <param name="bGreen">Green component of a pixel</param>
		/// <param name="bBlue">Blue component of a pixel</param>
		/// <returns></returns>
		public string getClass(Color c){
		  // create a unique name for this colour by using the RGB
		  // values with dashes in between so that 2 255 0 isn't
		  // confused with 22 55 0 or 225 5 0 or 22 5 50, etc.
		  // We also want to use something that is a valid CSS class name.
		  string strName = c.R.ToString() + 
		                   "-" + 
		                   c.G.ToString() +
		                   "-" +
		                   c.B.ToString();
		                   
		  // enter the new CSS class if it doesn't already exist
		  if(!hashClasses.ContainsKey(strName)){
		    // create the CSS class string 
		    // (height is two because 1 skews the picture)
		    string strClass = "." + 
		                      strName + 
		                      " { background-color: rgb(" + 
		                      c.R.ToString() + 
		                      "," +
		                      c.G.ToString() +
		                      "," + 
		                      c.B.ToString() +
		                      "); width:1px; height:3px;}"; 
		                      
		    hashClasses.Add(strName, strClass);
		  }
		  
		  // return the name since it either already existed or was just created
		  return strName;
		}
		
		/// <summary>
		///   Return a CSS style sheet with all the styles we've created
		/// </summary>
		/// <returns></returns>
		public string getStyleString(){
		  // get all the hash keys
		  ICollection keys = hashClasses.Keys;
		  StringBuilder bCSS = new StringBuilder();
		  
		  // start the CSS style tag
		  bCSS.Append("<style>");
		  
		  // add a table class to collaps the borders
		  bCSS.Append("  .table {border-collapse: collapse; border: 1px solid black;}");
		  
		  // for each hash key get the CSS class string
		  // and append it to the HTML stringbuilder
		  foreach(object key in keys){
		    bCSS.Append("  " + (string)hashClasses[key]);
		  }
		  
		  // end the CSS style tag
		  bCSS.Append("</style>");
		  
		  // return the CSS <style> sheet HTML
		  return bCSS.ToString();
		}
	}
}
