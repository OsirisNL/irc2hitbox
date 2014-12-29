/*
 *  The ircd.net project is an IRC deamon implementation for the .NET Plattform
 *  It should run on both .NET and Mono
 * 
 *  Copyright (c) 2009-2010 Thomas Bruderer <apophis@apophis.ch>
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */


using System.Text;
using IrcD.Modes;
using IrcD.Commands.Arguments;
using Hitbox;


namespace IrcD.Channel
{
    public class UserPerChannelInfo : InfoBase
    {
        private readonly UserInfo userInfo;
        public HitboxWebsocket hws;

        public UserInfo UserInfo
        {
            get
            {
                return userInfo;
            }
        }

        private readonly ChannelInfo channelInfo;

        public ChannelInfo ChannelInfo
        {
            get
            {
                return channelInfo;
            }
        }

        public UserPerChannelInfo(UserInfo userInfo, ChannelInfo channelInfo)
            : base(userInfo.IrcDaemon)
        {
            this.userInfo = userInfo;
            this.channelInfo = channelInfo;
            modes = new RankList(userInfo.IrcDaemon);

            
            hws = new HitboxWebsocket("ws://" + HitboxHelper.GetServer() + "/socket.io/1/websocket/" + HitboxHelper.GetIdentityId());
            hws.Username = userInfo.Nick;
            hws.Password = userInfo.Password;
            //IrcD.Utils.Logger.Log("Username: " + userInfo.Nick + " Password: " + userInfo.Password);

            hws.Connect(channelInfo.Name.Remove(0, 1));
            hws.OnError += (sender, e) =>
                {
                    var i = 0;
                };
            
            hws.OnMessage += (sender, e) =>
                {
                    foreach (UserInfo user in channelInfo.Users)
                    {
                       // if ( user != userInfo )
                            //if ( e.Data.Contains("buffer"))
                                IrcDaemon.Commands.Send(new PrivateMessageArgument(user, channelInfo, channelInfo.Name, "test"));
                    }
                    
                    //this.WriteLine(new StringBuilder("test"));
                };
        }


        private readonly RankList modes;

        public RankList Modes
        {
            get
            {
                return modes;
            }
        }

        /// <summary>
        /// This method just delegates the work to 
        /// </summary>
        /// <param name="line"></param>
        public override int WriteLine(StringBuilder line)
        {
            return userInfo.WriteLine(line);
        }

        public override int WriteLine(StringBuilder line, UserInfo exception)
        {
            if (UserInfo != exception)
            {
                return userInfo.WriteLine(line);
            }
            return 0;
        }
    }
}
