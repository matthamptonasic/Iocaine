using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Iocaine2.Data.Client;
using Iocaine2.Inventory;
using Iocaine2.Logging;
using Iocaine2.Memory;
using Iocaine2.Memory.Interface;
using Iocaine2.Tools;

namespace Iocaine2.Bots
{
    public abstract class BotParam
    {
        public Iocaine_2_Form.BOT BotType = Iocaine_2_Form.BOT.UNKNOWN;
        public Button StartButton = null;
        public Button StopButton = null;
        public Label TimerLabel = null;
        public TextBox StatusBox = null;
    }

    public sealed class FisherParam : BotParam
    {
        public FisherParam()
        {
            BotType = Iocaine_2_Form.BOT.FISHER;
        }
        public System.Windows.Forms.Button ReleaseButton = null;
        public System.Windows.Forms.TextBox TimeDateTB = null;
        public System.Windows.Forms.TextBox DayTB = null;
        public System.Windows.Forms.TextBox MoonTB = null;
        public System.Windows.Forms.TextBox WeatherTB = null;
        public System.Windows.Forms.Label ZoneLabel = null;
        public System.Windows.Forms.Label LastCatchLabel = null;
        public System.Windows.Forms.Label FishNameLabel = null;
        public System.Windows.Forms.Label CurHPLabel = null;
        public System.Windows.Forms.Label MaxHPLabel = null;
        public System.Windows.Forms.Label FatigueLabel = null;
        public System.Windows.Forms.Label Id1Label = null;
        public System.Windows.Forms.Label Id2Label = null;
        public System.Windows.Forms.Label Id3Label = null;
        public System.Windows.Forms.Label LargeLabel = null;
        public System.Windows.Forms.ProgressBar HpProgressBar = null;
        public System.Windows.Forms.CheckedListBox DropListBox = null;
        public System.Windows.Forms.TextBox DropItemTB = null;
        public System.Windows.Forms.Button DropBoxAddButton = null;
        public System.Windows.Forms.Button DropBoxRemoveButton = null;
        public System.Windows.Forms.CheckedListBox BaitListBox = null;
        public System.Windows.Forms.Button BaitBoxUpButton = null;
        public System.Windows.Forms.Button BaitBoxDownButton = null;
        public System.Windows.Forms.Button BaitBoxRefreshButton = null;
        public System.Windows.Forms.CheckedListBox FishListBox = null;
        public System.Windows.Forms.ListView StatsListBox = null;
        public System.Windows.Forms.Label RodName = null;
        public System.Windows.Forms.Label BaitName = null;
        public System.Windows.Forms.Label BaitQuan = null;
        public System.Windows.Forms.PictureBox LeftArrow = null;
        public System.Windows.Forms.PictureBox RightArrow = null;
        public System.Windows.Forms.CheckBox AutoResetChkB = null;
        public System.Windows.Forms.CheckBox ThisZoneOnlyChkB = null;
        public Statics.FuncPtrs.TD_Void_Void FishingDoneHandler = null;
        public Statics.FuncPtrs.TD_Void_Void FisherFatiguedHandler = null;
    }
    public sealed class PLParam : BotParam
    {
        public PLParam()
        {
            BotType = Iocaine_2_Form.BOT.PL;
        }
    }
    public sealed class SUParam : BotParam
    {
        public SUParam()
        {
            BotType = Iocaine_2_Form.BOT.SU;
        }
        public ListBox CommandListBox = null;
        public Byte CurrentSkil = 0;
        public Boolean GoToCap = false;
        public Int32 SkillLevel = 0;
        public Boolean StopAtGivenSkill = false;
        public Boolean DoOnly = false;
        public Int32 DoOnlyCount = 0;
        public Boolean DoneStop = false;
        public Boolean DoneLogout = false;
        public Boolean DoneShutdown = false;
        public Boolean GiveLogoutCommand = false;
        public String LogoutCommand = "";
        public Boolean GiveRestCommand = false;
        public String RestCommand = "";
        public Boolean DelayBetweenCasts = false;
        public UInt32 DelayValueBetweenCasts = 0;
        public Iocaine_2_Form.SU_SetSUStartButtonDelegate SetStartButtonCallBack = null;
    }
    public sealed class CrafterParam : BotParam
    {
        public CrafterParam()
        {
            BotType = Iocaine_2_Form.BOT.CRAFTER;
        }
    }
    public sealed class TAParam : BotParam
    {
        public TAParam()
        {
            BotType = Iocaine_2_Form.BOT.TA;
        }
    }
    public sealed class NavParam : BotParam
    {
        public NavParam()
        {
            BotType = Iocaine_2_Form.BOT.NAV;
        }
    }
    public sealed class TraderParam : BotParam
    {
        public TraderParam()
        {
            BotType = Iocaine_2_Form.BOT.TRADER;
        }
    }
    public sealed class BuyerParam : BotParam
    {
        public BuyerParam()
        {
            BotType = Iocaine_2_Form.BOT.BUYER;
        }
    }
    public sealed class SellerParam : BotParam
    {
        public SellerParam()
        {
            BotType = Iocaine_2_Form.BOT.SELLER;
        }
    }
}
