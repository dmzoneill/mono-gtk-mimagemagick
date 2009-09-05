using System;
using Gtk;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public partial class MainWindow: Gtk.Window
{	
	
	private System.Diagnostics.Process processInstance;
	private StreamWriter avimergeStreamWriter;
	private TextBuffer outbuffer;
	private TextBuffer errorbuffer;

	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{		
		Build ();
			
		string opts = "‑adaptive‑blur,‑adaptive‑resize,‑adaptive‑sharpen,‑adjoin,‑affine,‑alpha,‑annotate,‑antialias,‑append,‑attenuate,‑authenticate,‑auto‑gamma,‑auto‑level,‑auto‑orient,‑average,‑backdrop,‑background,‑bench,‑bias,‑blackpoint‑compensation,‑black‑threshold,‑blend,‑blue‑primary,‑blue‑shift,‑blur,‑border,‑bordercolor,‑borderwidth,‑cache,‑caption,‑cdl,‑channel,‑charcoal,‑chop,‑clip,‑clip‑mask,‑clip‑path,‑clone,‑clut,‑coalesce,‑colorize,‑colormap,‑colors,‑colorspace,‑combine,‑comment,‑compose,‑composite,‑compress,‑contrast,‑contrast‑stretch,‑convolve,‑crop,‑cycle,‑debug,‑decipher,‑deconstruct,‑define,‑delay,‑delete,‑density,‑depth,‑descend,‑deskew,‑despeckle,‑displace,‑display,‑dispose,‑dissimilarity‑threshold,‑dissolve,‑distort,‑dither,‑draw,‑edge,‑emboss,‑encipher,‑encoding,‑endian,‑enhance,‑equalize,‑evaluate,‑extent,‑extract,‑family,‑fft,‑fill,‑filter,‑flatten,‑flip,‑floodfill,‑flop,‑font,‑foreground,‑format,‑format[identify],‑frame,‑frame[import],‑function,‑fuzz,‑fx,‑gamma,‑gaussian‑blur,‑geometry,‑gravity,‑green‑primary,‑hald‑clut,‑help,‑highlight‑color,‑iconGeometry,‑iconic,‑identify,‑ift,‑immutable,‑implode,‑insert,‑intent,‑interlace,‑interpolate,‑interword‑spacing,‑kerning,‑label,‑lat,‑layers,‑level,‑level‑colors,‑limit,‑linear‑stretch,‑linewidth,‑liquid‑rescale,‑list,‑log,‑loop,‑lowlight‑color,‑magnify,‑map,‑map[stream],‑mask,‑mattecolor,‑median,‑metric,‑mode,‑modulate,‑monitor,‑monochrome,‑morph,‑mosaic,‑motion‑blur,‑name,‑negate,‑noise,‑normalize,‑opaque,‑ordered‑dither,‑orient,‑page,‑paint,‑path,‑pause[animate],‑pause[import],‑pen,‑ping,‑pointsize,‑polaroid,‑posterize,‑preview,‑print,‑process,‑profile,‑quality,‑quantize,‑quiet,‑radial‑blur,‑raise,‑random‑threshold,‑recolor,‑red‑primary,‑regard‑warnings,‑region,‑remap,‑remote,‑render,‑repage,‑resample,‑resize,‑respect‑parentheses,‑reverse,‑roll,‑rotate,‑sample,‑sampling‑factor,‑scale,‑scene,‑screen,‑seed,‑segment,‑selective‑blur,‑separate,‑sepia‑tone,‑set,‑shade,‑shadow,‑shared‑memory,‑sharpen,‑shave,‑shear,‑sigmoidal‑contrast,‑silent,‑size,‑sketch,‑snaps,‑solarize,‑sparse‑color,‑splice,‑spread,‑stegano,‑stereo,‑stretch,‑strip,‑stroke,‑strokewidth,‑style,‑swap,‑swirl,‑taint,‑text‑font,‑texture,‑threshold,‑thumbnail,‑tile,‑tile‑offset,‑tint,‑title,‑transform,‑transparent,‑transparent‑color,‑transpose,‑transverse,‑treedepth,‑trim,‑type,‑undercolor,‑unique‑colors,‑units,‑unsharp,‑update,‑verbose,‑version,‑view,‑vignette,‑virtual‑pixel,‑visual,‑watermark,‑wave,‑weight,‑white‑point,‑white‑threshold,‑window,‑window‑group,‑write";
		string[] options = opts.Split(',');
		
		foreach(string option in options)
		{
			this.comboboxentry1.AppendText(option);	
		}
		
		this.image1.Pixbuf = new Gdk.Pixbuf("in.jpg");
		
		outbuffer = this.textview1.Buffer;
		errorbuffer = this.textview2.Buffer;
				
		foreach(object font in this.getFonts())
		{
			string[] fontdetails = (string[]) font;
			//Console.WriteLine(fontdetails[0] + " - " + fontdetails[1] + " - " + fontdetails[2]);
			this.comboboxentry2.AppendText(fontdetails[0]);
		}
		
	}
	
	
	public ArrayList getFonts()
	{
		// mkdir ~/.magick && touch  ~/.magick/type.xml && perl imagemagick-fonts.pl > ~/.magick/type.xml
		
		ArrayList list = new ArrayList();
		
		XmlDocument documentation = new XmlDocument();			
        documentation.Load(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/.magick/type.xml");
        XmlNodeList memberNodes = documentation.SelectNodes("//type");	
		
		string[] fontdetails;
		
        foreach (XmlNode node in memberNodes) 
		{
			fontdetails = new string[3];
			try
			{
				fontdetails[0] = node.Attributes["name"].Value;
			}			
			catch 
			{ 
				fontdetails[0] = ""; 
			}
			
			try
			{
				fontdetails[1] = node.Attributes["fullname"].Value;
			}			
			catch 
			{ 
				fontdetails[1] = ""; 
			}
			
			try
			{
				fontdetails[2] = node.Attributes["glyphs"].Value;
			}			
			catch 
			{ 
				fontdetails[2] = ""; 
			}			
			
			list.Add(fontdetails);			
        }
		
		return list;
	}
	
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	
	protected virtual void OnButton1Clicked (object sender, System.EventArgs e)
	{
		if(File.Exists("/home/dave/Projects/Mimagemagick/Mimagemagick/bin/Debug/out.jpg"))
		{
			File.Delete("/home/dave/Projects/Mimagemagick/Mimagemagick/bin/Debug/out.jpg");	
		}
		
		this.image1.Pixbuf = null;
		
		string infile = this.entry1.Text;
		string outfile = this.entry2.Text;
		string options = this.entry3.Text;		
		
		this.processInstance = new Process();
        this.processInstance.StartInfo.FileName = "convert";
       	this.processInstance.StartInfo.Arguments = options + " \"" + infile + "\" \"" + outfile +"\"";				
       	this.processInstance.StartInfo.UseShellExecute = false;
        this.processInstance.StartInfo.RedirectStandardOutput = true;
		this.processInstance.OutputDataReceived += HandleOutputDataReceived;
		this.processInstance.StartInfo.RedirectStandardInput = true;
		this.processInstance.Start();
				
		this.avimergeStreamWriter = this.processInstance.StandardInput;
		this.avimergeStreamWriter.AutoFlush = true;
		this.processInstance.BeginOutputReadLine();				   		
		this.processInstance.WaitForExit();

		this.image1.Pixbuf = new Gdk.Pixbuf("/home/dave/Projects/Mimagemagick/Mimagemagick/bin/Debug/out.jpg");
	}
	
	
	protected void HandleOutputDataReceived(object sender, DataReceivedEventArgs e)
	{
		this.outbuffer.Text += e.Data.ToString();
	}
	
	
	protected void HandleErrorDataReceived(object sender, DataReceivedEventArgs e)
	{
		this.errorbuffer.Text += e.Data.ToString();
	}

	
	protected virtual void OnButton2Clicked (object sender, System.EventArgs e)
	{
		string option = this.comboboxentry1.ActiveText.ToString();
		string val = this.entry4.Text;
		this.entry3.Text = this.entry3.Text + " " + option + " " + val;		
	}

	
	protected virtual void OnButton3Clicked (object sender, System.EventArgs e)
	{
		// convert flower.jpg -font courier -fill white -pointsize 20 -annotate +50+50 'Flower' flower_annotate1.jpg
		string text = this.entry5.Text;
		string fontname = this.comboboxentry2.ActiveText;		
		string fontcolor = String.Format("#{0:X2}{1:X2}{2:X2}", (byte)(this.colorbutton2.Color.Red >> 8), (byte)(this.colorbutton2.Color.Green >> 8), (byte)(this.colorbutton2.Color.Blue >> 8));
		string fontsize = this.spinbutton1.Value.ToString();
		string xpos = this.entry7.Text;
		string ypos = this.entry8.Text;
		
		this.entry3.Text += " -font '" + fontname + "' -fill '" + fontcolor + "' -pointsize " + fontsize + " -annotate +" + xpos + "+" + ypos + " '" + text + "'";
	}

	protected virtual void OnImage1ButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
	{
		Gtk.Image icon = new Gtk.Image(Stock.MissingImage, IconSize.LargeToolbar);
		
		if(this.image1.Pixbuf.Equals(icon.Pixbuf))
		{
			Console.WriteLine("clicked");	
		}
	}
	
}