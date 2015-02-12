using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;
using Grabacr07.KanColleWrapper;
using System.Windows;

namespace HoppoPlugin
{
    public class Counter : NotificationObject
    {
        public bool EnableLogging = true;
        //private bool waitingForShip;
        //private int dockid;
        //private readonly int[] shipmats;
        //private readonly string LogTimestampFormat = "yyyy-MM-dd HH:mm:ss";

        private enum LogType
        {
            BuildItem,
            BuildShip,
            ShipDrop,
            Materials
        };

        //initialize 

        internal Counter(KanColleProxy proxy)
        {
            proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>().Subscribe(x => this.BattleResult(x.Data));

        }

        private void BattleResult(kcsapi_battleresult br)
        {
            if(br.api_quest_level != null)
            {
                StorieResult++;
            }
            //MessageBox.Show("api_enemy_info.api_level " + br.api_enemy_info.api_level);
            //MessageBox.Show("api_quest_level " + br.api_quest_level.ToString());
            //if (br.api_get_ship == null)
            //    return;

            //Log(LogType.ShipDrop, "{0},{1},{2},{3},{4}", DateTime.Now.ToString(this.LogTimestampFormat),
            //    br.api_get_ship.api_ship_name,
            //    br.api_quest_name,
            //    br.api_enemy_info.api_deck_name,
            //    br.api_win_rank);
        }

        #region StorieResult 変更通知プロパティ

        private int _StorieResult;

        public int StorieResult
        {
            get { return this._StorieResult; }
            set
            {
                if (this._StorieResult != value)
                {
                    this._StorieResult = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region BossStorieResult 変更通知プロパティ

        private int _BossStorieResult;

        public int BossStorieResult
        {
            get { return this._BossStorieResult; }
            set
            {
                if (this._BossStorieResult != value)
                {
                    this._BossStorieResult = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

    }
}
