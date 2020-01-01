using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System.Xml;
using System.IO;
using System.Drawing;

namespace ChannelPointsDisplay
{
	public partial class ChannelPointsDisplay : Form
	{
		private RemoteWebDriver Driver;
		private string ChannelName;
		private Dictionary<string, string> RedemptionNames = new Dictionary<string, string> {
			{ "img", "" },
			{ "gif", "" }
		};
		private Timer ContentChangedTimer = new Timer();

		private const string ChatElementClassName = "chat-list__lines";

		public ChannelPointsDisplay()
		{
			InitializeComponent();

			LoadSettings();

			//Hide console
			ChromeDriverService ChromeService = ChromeDriverService.CreateDefaultService();
			ChromeService.HideCommandPromptWindow = true;
			#if DEBUG
			ChromeService.HideCommandPromptWindow = false;
			#endif

			//Hide browser
			ChromeOptions Options = new ChromeOptions();
			Options.AddArgument("headless");

			//Start driver
			Driver = new ChromeDriver(ChromeService, Options);
			Driver.Navigate().GoToUrl("https://www.twitch.tv/embed/" + ChannelName + "/chat");

			//Start MutationObserver to watch for element additions
			StartWatchingForContentChange(ChatElementClassName);

			ContentChangedTimer.Tick += new EventHandler(TryRedemptionsTick);
			ContentChangedTimer.Interval = 1000 * 3; //Every 3 seconds
			ContentChangedTimer.Start();
		}

		private void LoadSettings()
		{
			XmlDocument SettingsDocument = new XmlDocument();
			SettingsDocument.Load("settings.xml");

			//Retrieve name of the channel to watch the chat for redemptions
			ChannelName = SettingsDocument.SelectSingleNode("/Settings/ChannelName").InnerText.ToLower();

			//Retrieve redemption names to watch for
			XmlNodeList NodeList = SettingsDocument.SelectNodes("/Settings/Redemption");
			foreach(XmlNode Node in NodeList)
			{
				string Type = Node.Attributes["Type"].InnerText.ToLower();
				if(RedemptionNames.ContainsKey(Type))
				{
					RedemptionNames[Type] = Name = Node.InnerText.ToLower();
				}
			}
		}

		private void TryRedemptionsTick(object sender, EventArgs e)
		{
			TryRedemptions();
		}

		private void TryRedemptions()
		{
			List<string> NewContent = GetNewContent(ChatElementClassName);
			//NewContent.Add("justinfan56: message test");
			//NewContent.Add("redeemed highlight my message\r\n100\r\justinfan56: highlight test");
			//NewContent.Add("redeemed video request\r\n100\r\njustinfan56: video test");
			//NewContent.Add("justinfan56 redeemed test\r\n100");
			//NewContent.Add("justinfan56 redeemed show random image\r\n100");

			for(int i = 0 ; i < NewContent.Count ; ++i)
			{
				const string RedeemedString = "redeemed ";
				int RedeemedIndexStart = NewContent[i].IndexOf(RedeemedString);
				if(RedeemedIndexStart != -1)
				{
					int RedeemedIndexEnd = NewContent[i].IndexOf("\r\n");
					int CharactersToRedemption = RedeemedIndexStart + RedeemedString.Length;
					string Redemption = NewContent[i].Substring(CharactersToRedemption, RedeemedIndexEnd - CharactersToRedemption).ToLower();

					if(RedemptionNames["img"] == Redemption)
					{
						DisplayRandomImage();
					}
					else if(RedemptionNames["gif"] == Redemption)
					{
						DisplayRandomGIF();
					}
				}
			}
		}

		private void StartWatchingForContentChange(string containerID)
		{
			//Retry if there is an exception (https://stackoverflow.com/questions/12967541/how-to-avoid-staleelementreferenceexception-in-selenium)
			int Attempts = 0;
			const int MaxAttempts = 20;
			while(Attempts < MaxAttempts)
			{
				try
				{
					//Add a mutation observer to watch for element additions and add them to a list for later
					Driver.ExecuteScript(@"var $$expectedId = arguments[0];
						__selenium_observers__ =  window.__selenium_observers__ || {};

						(function(){        
						var target = document.getElementsByClassName($$expectedId)[0];
						__selenium_observers__[$$expectedId] = {
								observer: new MutationObserver(function(mutations) {
									__selenium_observers__[$$expectedId].mutations.push(mutations);
								})
						};
						__selenium_observers__[$$expectedId].mutations = [];

						var config = { attributes: true, childList: true, characterData: true, subtree: true };
						__selenium_observers__[$$expectedId].observer.observe(target, config);
						})();", containerID);

					break;
				}
				catch(OpenQA.Selenium.StaleElementReferenceException e)
				{
				}

				++Attempts;
			}

			if(Attempts == MaxAttempts)
			{
				throw new Exception("Could not set mutation observer");
			}
		}

		private List<string> GetNewContent(string containerID)
		{
			List<string> NewContent = new List<string>();
			object Mutations = null;

			//Retry if there is an exception (https://stackoverflow.com/questions/12967541/how-to-avoid-staleelementreferenceexception-in-selenium)
			int Attempts = 0;
			const int MaxAttempts = 20;
			while(Attempts < MaxAttempts)
			{
				try
				{
					//Get all the mutations that occured since last call and empty the list
					Mutations = Driver.ExecuteScript(@"
						var mutations = [...window.__selenium_observers__[arguments[0]].mutations];
						window.__selenium_observers__[arguments[0]].mutations = [];
						return mutations;", containerID);
					break;
				}
				catch(OpenQA.Selenium.StaleElementReferenceException e)
				{
				}

				++Attempts;
			}

			if(Attempts == MaxAttempts)
			{
				return NewContent;
			}

			ReadOnlyCollection<object> MutationsList = (ReadOnlyCollection<object>)Mutations;

			//Retrieve the text of the elements that were added
			for(int i = 0 ; i < MutationsList.Count ; ++i)
			{
				var Mutation1 = (ReadOnlyCollection<object>)MutationsList[i];
				var Mutation2 = Mutation1[0];
				var Additions = (Dictionary<string, object>)Mutation2;
				var AddedNodes = Additions["addedNodes"];
				var Elements = AddedNodes as ReadOnlyCollection<IWebElement>;
				if(Elements != null)
				{
					NewContent.Add(Elements[0].Text.ToLower());
				}
			}

			return NewContent;
		}

		private string GetRandomFileFromFolder(string Folder)
		{
			string[] Images = Directory.GetFiles(Folder);
			if(Images.Length > 0)
			{
				Random Rand = new Random();
				string RandomFile = Images[Rand.Next(0, Images.Length)];

				return RandomFile;
			}

			return string.Empty;
		}

		private void DisplayRandomImage()
		{
			string RandomImage = GetRandomFileFromFolder("images");
			if(File.Exists(RandomImage))
			{
				ImageBox.Image = Image.FromFile(RandomImage);
				ImageToTopLeft();
			}
		}

		private void DisplayRandomGIF()
		{
			string RandomGIF = GetRandomFileFromFolder("gifs");
			if(File.Exists(RandomGIF))
			{
				ImageBox.Image = Image.FromFile(RandomGIF);
				ImageToTopLeft();
			}
		}

		private void DisplayRandomVideo()
		{
			string RandomVideo = GetRandomFileFromFolder("videos");
			if(File.Exists(RandomVideo))
			{
				VideoBox.URL = RandomVideo;
				VideoBox.uiMode = "none";
			}
		}

		private void ImageToTopLeft()
		{
			//Reset position and size
			ImageBox.Location = new Point(0, 0);
			ImageBox.Dock = DockStyle.Fill;
			Size NewSize = ImageBox.Size;
			ImageBox.Dock = DockStyle.None;
			ImageBox.Size = NewSize;

			Size ImageSize = ImageBox.Image.Size;
			float ImageRatio = (float)ImageSize.Width / ImageSize.Height;
			float ImageBoxRatio = (float)ImageBox.Size.Width / ImageBox.Size.Height;
			
			if(ImageRatio > ImageBoxRatio)
			{
				float ImageHeight = ImageBox.Size.Width / ImageRatio;

				Point NewLocation = ImageBox.Location;
				int MoveDelta = ImageBox.Size.Height - (int)ImageHeight;
				NewLocation.Y -= MoveDelta;
				ImageBox.Location = NewLocation;

				Size BoxSize = ImageBox.Size;
				BoxSize.Height += MoveDelta;
				ImageBox.Size = BoxSize;
			}
			else
			{
				float ImageWidth = ImageBox.Size.Height * ImageRatio;

				Point NewLocation = ImageBox.Location;
				int MoveDelta = ImageBox.Size.Width - (int)ImageWidth;
				NewLocation.X -= MoveDelta;
				ImageBox.Location = NewLocation;

				Size BoxSize = ImageBox.Size;
				BoxSize.Width += MoveDelta;
				ImageBox.Size = BoxSize;
			}
		}

		//For testing
		private void ImageBox_MouseDown(object sender, MouseEventArgs e)
		{
			//DisplayRandomImage();
			//DisplayRandomGIF();
		}
	}
}
