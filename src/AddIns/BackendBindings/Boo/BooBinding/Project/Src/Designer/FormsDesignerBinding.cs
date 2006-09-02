// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace Grunwald.BooBinding.Designer
{
	public class FormsDesignerDisplayBinding : ISecondaryDisplayBinding
	{
		/// <summary>
		/// When you return true for this property, the CreateSecondaryViewContent method
		/// is called again after the LoadSolutionProjects thread has finished.
		/// </summary>
		public bool ReattachWhenParserServiceIsReady {
			get {
				return true;
			}
		}
		
		public bool CanAttachTo(IViewContent viewContent)
		{
			if (viewContent is ITextEditorControlProvider) {
				ITextEditorControlProvider textAreaControlProvider = (ITextEditorControlProvider)viewContent;
				string fileExtension = String.Empty;
				string fileName      = viewContent.IsUntitled ? viewContent.UntitledName : viewContent.FileName;
				if (fileName == null)
					return false;
				if (Path.GetExtension(fileName).Equals(".boo", StringComparison.InvariantCultureIgnoreCase)) {
					ParseInformation info = ParserService.ParseFile(fileName, textAreaControlProvider.TextEditorControl.Document.TextContent, false, true);
					if (FormsDesignerSecondaryDisplayBinding.IsDesignable(info))
						return true;
				}
			}
			return false;
		}
		
		public ISecondaryViewContent[] CreateSecondaryViewContent(IViewContent viewContent)
		{
			if (viewContent.SecondaryViewContents.Exists(delegate(ISecondaryViewContent c) { return c.GetType() == typeof(FormsDesignerViewContent); })) {
				return new ISecondaryViewContent[0];
			}
			
			IDesignerLoaderProvider loader = new BooDesignerLoaderProvider(((ITextEditorControlProvider)viewContent).TextEditorControl);
			IDesignerGenerator generator = new BooDesignerGenerator();
			return new ISecondaryViewContent[] { new FormsDesignerViewContent(viewContent, loader, generator) };
		}
	}
	
	public class BooDesignerLoaderProvider : IDesignerLoaderProvider
	{
		TextEditorControl textEditorControl;
		
		public BooDesignerLoaderProvider(TextEditorControl textEditorControl)
		{
			this.textEditorControl = textEditorControl;
		}
		
		public System.ComponentModel.Design.Serialization.DesignerLoader CreateLoader(IDesignerGenerator generator)
		{
			return new BooDesignerLoader(textEditorControl, generator);
		}
	}
}
