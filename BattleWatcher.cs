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
    public class BattleWatcher : NotificationObject
    {
        //public bool EnableLogging = true;
        //private bool waitingForShip;
        //private int dockid;
        //private readonly int[] shipmats;
        //private readonly string LogTimestampFormat = "yyyy-MM-dd HH:mm:ss";

        //private enum LogType
        //{
        //    BuildItem,
        //    BuildShip,
        //    ShipDrop,
        //    Materials
        //};

        //initialize 

        //internal BattleWatcher(KanColleProxy proxy)
        //{
        //    proxy.api_req_sortie_battle.TryParse<kcsapi_battle>().Subscribe(x => this.BattleResultWatcher(x.Data.apiData));
        //}

        //private void BattleResultWatcher(Api_Data br)
        //{
        //    MessageBox.Show("first");
        //    //foreach(int shipLV in br.api_ship_lv)
        //    //{
        //    //    _BattleResult += "\n" + shipLV.ToString();
        //    //}
        //    foreach(int shipLV in br.api_ship_lv)
        //    {
        //        MessageBox.Show("second");
        //        MessageBox.Show(shipLV.ToString());
        //        BattleResult += "\n" + shipLV.ToString();
        //    }
            
        //    //_BattleResult += br.api_deck_id;
        //    //_BattleResult += br.api_eKyouka;
        //    //_BattleResult += br.api_eParam;
        //    //_BattleResult += br.api_eSlot;
        //    //_BattleResult += br.api_formation;
        //    //_BattleResult += br.api_fParam;
        //    //_BattleResult += br.api_fParam_combined;
        //    //_BattleResult += br.api_hougeki1;
        //    //_BattleResult += br.api_hougeki2;
        //    //_BattleResult += br.api_hougeki3;
        //    //_BattleResult += br.api_hourai_flag;
        //    //_BattleResult += br.api_kouku;
        //    //_BattleResult += br.api_maxhps;
        //    //_BattleResult += br.api_maxhps_combined;
        //    //_BattleResult += br.api_midnight_flag;
        //    //_BattleResult += br.api_nowhps;
        //    //    _BattleResult += br.api_nowhps_combined;
        //    //        _BattleResult += br.api_opening_atack;
        //    //            _BattleResult += br.api_opening_flag;
        //    //                _BattleResult += br.api_raigeki;
        //    //                    _BattleResult += br.api_search;
        //    //                    _BattleResult += br.api_ship_ke;
        //    //                    _BattleResult += br.api_ship_lv; 
        //    //_BattleResult += br.api_stage_flag;
        //    //    _BattleResult += br.api_support_flag;
        //    //        _BattleResult += br.api_support_info;
        //    //_BattleResult += br.

        //    //if (br.api_quest_level != null)
        //    //{
        //    //    StorieResult++;
        //    //}
        //    //MessageBox.Show("api_enemy_info.api_level " + br.api_enemy_info.api_level);
        //    //MessageBox.Show("api_quest_level " + br.api_quest_level.ToString());
        //    //if (br.api_get_ship == null)
        //    //    return;

        //    //Log(LogType.ShipDrop, "{0},{1},{2},{3},{4}", DateTime.Now.ToString(this.LogTimestampFormat),
        //    //    br.api_get_ship.api_ship_name,
        //    //    br.api_quest_name,
        //    //    br.api_enemy_info.api_deck_name,
        //    //    br.api_win_rank);
        //}


        #region BattleResult 変更通知プロパティ

        private string _BattleResult = "No Result";

        public string BattleResult
        {
            get { return this._BattleResult; }
            set
            {
                if (this._BattleResult != value)
                {
                    this._BattleResult = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

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

        //#region BossStorieResult 変更通知プロパティ

        //private int _BossStorieResult;

        //public int BossStorieResult
        //{
        //    get { return this._BossStorieResult; }
        //    set
        //    {
        //        if (this._BossStorieResult != value)
        //        {
        //            this._BossStorieResult = value;
        //            this.RaisePropertyChanged();
        //        }
        //    }
        //}

        //#endregion

    }
}
