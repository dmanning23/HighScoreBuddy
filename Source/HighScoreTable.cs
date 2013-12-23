using EasyStorage;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace HighScoreBuddy
{
	/// <summary>
	/// This is a list of all the lists used to store high scores for this game.
	/// </summary>
	public abstract class HighScoreTable
	{
		#region Member Variables

		/// <summary>
		/// The save device.
		/// </summary>
		private IAsyncSaveDevice saveDevice;
		
		/// <summary>
		/// Collection of all the high scores lists
		/// Maps the high score list name to the instance of the high score list.
		/// </summary>
		protected Dictionary<string, HighScoreList> HighScoreLists { get; private set; }

		/// <summary>
		/// The location to store this high score list.
		/// This will be a relative path from the user directory, so don't put in drive letters etc.
		/// </summary>
		/// <value>The name of the folder.</value>
		private string Folder { get; set; }

		/// <summary>
		/// Flag for whether or not teh high scores have been loaded from a file
		/// </summary>
		/// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
		private bool Loaded { get; set; }

		#endregion //Member Variables

		#region Methods

		/// <summary>
		/// hello standard constructor!
		/// </summary>
		public HighScoreTable(string FolderLocation)
		{
			//set the save location
			Folder = FolderLocation;

			HighScoreLists = new Dictionary<string, HighScoreList>();
			Loaded = false;
		}

		/// <summary>
		/// Adds all the lists to the table.
		/// </summary>
		protected abstract void AddLists();

		/// <summary>
		/// called once at the beginning of the program
		/// gets the storage device
		/// </summary>
		/// <param name="myGame">the current game.</param>
		public void Initialize(Game myGame)
		{
			//First add the default lists to the table
			AddLists();

			// on Windows Phone we use a save device that uses IsolatedStorage
			// on Windows and Xbox 360, we use a save device that gets a shared StorageDevice to handle our file IO.
#if WINDOWS_PHONE || ANDROID
			saveDevice = new IsolatedStorageSaveDevice();
#else
			// create and add our SaveDevice
			SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
			myGame.Components.Add(sharedSaveDevice);

			// make sure we hold on to the device
			saveDevice = sharedSaveDevice;

			// hook two event handlers to force the user to choose a new device if they cancel the
			// device selector or if they disconnect the storage device after selecting it
			sharedSaveDevice.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
			sharedSaveDevice.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;

			// prompt for a device on the first Update we can
			sharedSaveDevice.PromptForDevice();
#endif

#if XBOX
			// add the GamerServicesComponent
			Components.Add(new Microsoft.Xna.Framework.GamerServices.GamerServicesComponent(this));
#endif

			// hook an event so we can see that it does fire
			saveDevice.SaveCompleted += new SaveCompletedEventHandler(saveDevice_SaveCompleted);
		}
		
		/// <summary>
		/// event handler that gets fired off when a write op is completed
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		void saveDevice_SaveCompleted(object sender, FileActionCompletedEventArgs args)
		{
			//Write a message out to the deubg log so we know whats going on
			string strText = "SaveCompleted!";
			if (null != args.Error)
			{
				strText = args.Error.Message;
			}

			Debug.WriteLine(strText);
		}

		/// <summary>
		/// Gets the high score list by name
		/// </summary>
		/// <returns>The high score list.</returns>
		/// <param name="ListName">name of teh high score list to fetch.</param>
		public HighScoreList GetHighScoreList(string ListName)
		{
			if (HighScoreLists.ContainsKey(ListName))
			{
				return HighScoreLists[ListName];
			}
			else
			{
				//list does not exist
				return null;
			}
		}

		#endregion //Methods

		#region XML Methods

		/// <summary>
		/// Save all the high scores out to disk
		/// </summary>
		public void Save()
		{
			// make sure the device is ready
			if (saveDevice.IsReady)
			{
				// save a file asynchronously. this will trigger IsBusy to return true
				// for the duration of the save process.
				saveDevice.SaveAsync(
					Folder,
					"HighScores.xml",
					WriteHighScores);
			}
		}

		public void Load()
		{
			if (!Loaded)
			{
				try
				{
					//if there is a file there, load it into the system
					if (saveDevice.FileExists(Folder, "HighScores.xml"))
					{
						saveDevice.Load(
							Folder,
							"HighScores.xml",
							ReadHighScores);
					}

					//set the Loaded flag to true since high scores only need to be laoded once
					Loaded = true;
					Debug.WriteLine("Loaded High Scores");
				}
				catch (Exception ex)
				{
					//now you fucked up
					Loaded = false;

					// just write some debug output for our verification
					Debug.WriteLine(ex.Message);
				}
			}
		}

		/// <summary>
		/// do the actual writing out to disk
		/// </summary>
		/// <param name="myFileStream">My file stream.</param>
		private void WriteHighScores(Stream myFileStream)
		{
			try
			{
				//open the file, create it if it doesnt exist yet
				XmlTextWriter rFile = new XmlTextWriter(myFileStream, null);
				rFile.Formatting = Formatting.Indented;
				rFile.Indentation = 1;
				rFile.IndentChar = '\t';
				
				//save all the high scores!
				rFile.WriteStartDocument();

				//write the high score table element
				rFile.WriteStartElement("highscoretable");

				//write out all the lists
				foreach(KeyValuePair<string, HighScoreList> entry in HighScoreLists)
				{
					// do something with entry.Value or entry.Key
					entry.Value.WriteToXML(rFile);
				}

				rFile.WriteEndElement();

				rFile.WriteEndDocument();
				
				// Close the file.
				rFile.Flush();
				rFile.Close();
			}
			catch (Exception ex)
			{
				// just write some debug output for our verification
				Debug.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// to the actual reading in from disk
		/// </summary>
		/// <param name="myFileStream">My file stream.</param>
		private void ReadHighScores(Stream myFileStream)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(myFileStream);
			XmlNode rootNode = xmlDoc.DocumentElement;

			//make sure it is actually an xml node
			if (rootNode.NodeType == XmlNodeType.Element)
			{
				//make sure is correct type of node
				Debug.Assert("highscoretable" == rootNode.Name);

				//read high score lists
				if (rootNode.HasChildNodes)
				{
					XmlNode childNode = rootNode.FirstChild;
					while (childNode != null)
					{
						//get the name of this list
						string ListName = "";
						XmlNamedNodeMap mapAttributes = childNode.Attributes;
						for (int i = 0; i < mapAttributes.Count; i++)
						{
							//will only have the name attribute
							string strName = mapAttributes.Item(i).Name;
							string strValue = mapAttributes.Item(i).Value;
							if ("ListName" == strName)
							{
								ListName = strValue;
							}
							else
							{
								//unknwon attribute in the xml file!!!
								Debug.Assert(false);
							}
						}

						//find the list from the xml file
						Debug.Assert(!String.IsNullOrEmpty(ListName));
						HighScoreList rHighScoreList = HighScoreLists[ListName];
						Debug.Assert(null != rHighScoreList);

						//create, read, and store a new high score list from this xml node
						rHighScoreList.ReadFromXML(childNode);
						HighScoreLists[rHighScoreList.Name] = rHighScoreList;
						
						//next node!
						childNode = childNode.NextSibling;
					}
				}
			}
			else
			{
				//should be an xml node!!!
				Debug.Assert(false);
			}
		}

		#endregion //XML Methods
	}
}