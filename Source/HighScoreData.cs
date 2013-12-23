using System;
using System.Diagnostics;
using System.Xml;

namespace HighScoreBuddy
{
	/// <summary>
	/// this is an entry in one of the high scores tables
	/// </summary>
	public class HighScoreData
	{
		#region Members

		/// <summary>
		/// name of the player who earned this high score
		/// </summary>
		public string PlayerName { get; private set; }

		/// <summary>
		/// the actual high score/num lines/high combo
		/// </summary>
		public uint Score { get; private set; }

		/// <summary>
		/// flag for whether or not this high score was achieved this play session
		/// </summary>
		/// <value><c>true</c> if new high score; otherwise, <c>false</c>.</value>
		public bool NewHighScore { get; set; }

		#endregion //Members

		#region Initialization / Cleanup

		/// <summary>
		/// hello, default constructor!
		/// </summary>
		public HighScoreData()
		{
			NewHighScore = false;
		}
		
		/// <summary>
		/// create a high score data object.
		/// </summary>
		/// <param name="strName">name of the dude who got this high score.</param>
		/// <param name="iScore">the high score/most lines/highest combo that dude got</param>
		public HighScoreData(string strName, uint iScore) : this()
		{
			PlayerName = strName;
			Score = iScore;
		}

		#endregion //Initialization / Cleanup

		#region XML Methods

		/// <summary>
		/// read a high score entry from an xml node
		/// </summary>
		/// <param name="rXMLNode">R XML node.</param>
		public void ReadFromXML(XmlNode rXMLNode)
		{
			//make sure it is actually an xml node
			if (rXMLNode.NodeType == XmlNodeType.Element)
			{
				//make sure is correct type of node
				Debug.Assert("entry" == rXMLNode.Name);

				//get the name of this dude
				XmlNamedNodeMap mapAttributes = rXMLNode.Attributes;
				for (int i = 0; i < mapAttributes.Count; i++)
				{
					string strName = mapAttributes.Item(i).Name;
					string strValue = mapAttributes.Item(i).Value;
					if ("name" == strName)
					{
						PlayerName = strValue;
					}
					else if ("score" == strName)
					{
						Score = Convert.ToUInt32(strValue);
					}
					else
					{
						//unknwon attribute in the xml file!!!
						Debug.Assert(false);
					}
				}
			}
			else
			{
				//should be an xml node!!!
				Debug.Assert(false);
			}
		}

		public void WriteToXML(XmlTextWriter rXMLFile)
		{
			//write high score data element
			rXMLFile.WriteStartElement("entry");

			//write out the name
			rXMLFile.WriteAttributeString("name", PlayerName);

			//write out the score
			rXMLFile.WriteAttributeString("score", Score.ToString());

			rXMLFile.WriteEndElement();
		}

		#endregion //XML Methods
	}
}