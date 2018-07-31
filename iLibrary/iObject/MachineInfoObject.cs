using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace iLibrary
{
    #region MACHINE_UPDATE_INFO
    public class MachineUpdateInfo : MachineInfo
    {
        private bool c_IsOk;
        private int c_ErrorNo;
        private String c_ErrorMessage;
        private int c_User;

        public MachineUpdateInfo(int id, string name) : base(id, name)
        {
            c_IsOk = false;
            c_ErrorNo = -1;
            c_ErrorMessage = null;
            c_User = -1;
        }

        public bool IsOk
        {
            get { return c_IsOk; }
            set { c_IsOk = value; }
        }
        public int ErrorNo
        {
            get { return c_ErrorNo; }
            set { c_ErrorNo = value; }
        }
        public string ErrorMessage
        {
            get { return c_ErrorMessage; }
            set { c_ErrorMessage = value; }
        }
        public int User
        {
            get { return c_User; }
            set { c_User = value; }
        }
    }

    public class MachineComInfo : Info
    {
        private int c_model_id;
        private int c_comm_type;
        private Boolean c_is_active;
        private int c_sesc_t1;
        private int c_sesc_t2;
        private int c_sesc_t3;
        private int c_sesc_t4;
        private int c_sesc_t5;
        private int c_sesc_t6;
        private int c_sesc_t7;
        private int c_sesc_t8;
        //private int c_sesc_t9;
        //private int c_secs_tc;
        //private int c_secs_max_byte;
        private int c_secs_defalte_sid;
        private int c_secs_retry_limit;
        private Boolean c_secs_is_multi_block;
        private Boolean c_secs_rcv_double_block;
        private int c_rs232_baudrate;
        private int c_rs232_databits;
        private int c_rs232_paritybit;
        private int c_rs232_handshake;
        //private int c_rs232_rts_enable;
        private int c_rs232_stop_bits;
        private int c_handler_type;
        private List<CEID_RPTIDS> c_CEIDS;
        private List<RPTID_VIDS> c_RPTIDS;
        private List<Comm_Settings> c_Comms;

        public MachineComInfo() : base()
        {
            c_model_id = -1;
            c_comm_type = -1;
            c_is_active = false;
            c_sesc_t1 = -1;
            c_sesc_t2 = -1;
            c_sesc_t3 = -1;
            c_sesc_t4 = -1;
            c_sesc_t5 = -1;
            c_sesc_t6 = -1;
            c_sesc_t7 = -1;
            c_sesc_t8 = -1;
            //c_sesc_t9 = -1;
            //c_secs_tc = -1;
            //c_secs_max_byte = -1;
            c_secs_defalte_sid = -1;
            c_secs_retry_limit = -1;
            c_secs_is_multi_block = false;
            c_secs_rcv_double_block = false;
            c_rs232_baudrate = -1;
            c_rs232_databits = -1;
            c_rs232_paritybit = -1;
            c_rs232_handshake = -1;
            //c_rs232_rts_enable = -1;
            c_rs232_stop_bits = -1;
            c_handler_type = -1;
            c_CEIDS = null;
            c_RPTIDS = null;
            c_Comms = null;
        }

        public MachineComInfo(DataTable tmp) : base()
        {
            DB_GetConverter DBCONVERT = new DB_GetConverter();
            c_model_id = DBCONVERT.GetInt32(tmp.Rows[0], "machine_model_id");
            c_comm_type = DBCONVERT.GetInt32(tmp.Rows[0], "comm_type");
            c_is_active = DBCONVERT.GetBool(tmp.Rows[0], "is_active");
            c_sesc_t1 = DBCONVERT.GetInt32(tmp.Rows[0], "t1");
            c_sesc_t2 = DBCONVERT.GetInt32(tmp.Rows[0], "t2");
            c_sesc_t3 = DBCONVERT.GetInt32(tmp.Rows[0], "t3");
            c_sesc_t4 = DBCONVERT.GetInt32(tmp.Rows[0], "t4");
            c_sesc_t5 = DBCONVERT.GetInt32(tmp.Rows[0], "t5");
            c_sesc_t6 = DBCONVERT.GetInt32(tmp.Rows[0], "t6");
            c_sesc_t7 = DBCONVERT.GetInt32(tmp.Rows[0], "t7");
            c_sesc_t8 = DBCONVERT.GetInt32(tmp.Rows[0], "t8");
            //c_sesc_t9 = DBCONVERT.GetInt32(tmp.Rows[0], "t9"); 
            //c_secs_tc = DBCONVERT.GetInt32(tmp.Rows[0], "tc");
            //c_secs_max_byte = DBCONVERT.GetInt32(tmp.Rows[0], "tp_code");
            c_secs_defalte_sid = DBCONVERT.GetInt32(tmp.Rows[0], "default_sid");
            c_secs_retry_limit = DBCONVERT.GetInt32(tmp.Rows[0], "retry_limit");
            c_secs_is_multi_block = DBCONVERT.GetBool(tmp.Rows[0], "is_multi_block");
            c_secs_rcv_double_block = DBCONVERT.GetBool(tmp.Rows[0], "is_allow_double_block");
            c_rs232_baudrate = DBCONVERT.GetInt32(tmp.Rows[0], "rs232c_baud_rate");
            c_rs232_databits = DBCONVERT.GetInt32(tmp.Rows[0], "rs232c_data_bits");
            c_rs232_paritybit = DBCONVERT.GetInt32(tmp.Rows[0], "rs232c_parity_bit");
            c_rs232_handshake = DBCONVERT.GetInt32(tmp.Rows[0], "rs232c_handshake");
            //c_rs232_rts_enable = DBCONVERT.GetInt32(tmp.Rows[0], "tp_code");
            c_rs232_stop_bits = DBCONVERT.GetInt32(tmp.Rows[0], "rs232c_stop_bit");
            c_handler_type = DBCONVERT.GetInt32(tmp.Rows[0], "handler_type");
            c_CEIDS = null;
            c_RPTIDS = null;
            c_Comms = null;
        }
        public int model_id
        {
            get { return c_model_id; }
            set { c_model_id = value; }
        }
        public int comm_type
        {
            get { return c_comm_type; }
            set { c_comm_type = value; }
        }
        public Boolean is_active
        {
            get { return c_is_active; }
            set { c_is_active = value; }
        }
        public int sesc_t1
        {
            get { return c_sesc_t1; }
            set { c_sesc_t1 = value; }
        }
        public int sesc_t2
        {
            get { return c_sesc_t2; }
            set { c_sesc_t2 = value; }
        }
        public int sesc_t3
        {
            get { return c_sesc_t3; }
            set { c_sesc_t3 = value; }
        }
        public int sesc_t4
        {
            get { return c_sesc_t4; }
            set { c_sesc_t4 = value; }
        }
        public int sesc_t5
        {
            get { return c_sesc_t5; }
            set { c_sesc_t5 = value; }
        }
        public int sesc_t6
        {
            get { return c_sesc_t6; }
            set { c_sesc_t6 = value; }
        }
        public int sesc_t7
        {
            get { return c_sesc_t7; }
            set { c_sesc_t7 = value; }
        }
        public int sesc_t8
        {
            get { return c_sesc_t8; }
            set { c_sesc_t8 = value; }
        }
        //public int sesc_t9
        //{
        //    get { return c_sesc_t9; }
        //    set { c_sesc_t9 = value; }
        //}
        //public int secs_max_byte
        //{
        //    get { return c_secs_max_byte; }
        //    set { c_secs_max_byte = value; }
        //}
        //public int sesc_tc
        //{
        //    get { return c_secs_tc; }
        //    set { c_secs_tc = value; }
        //}
        public int secs_defalte_sid
        {
            get { return c_secs_defalte_sid; }
            set { c_secs_defalte_sid = value; }
        }
        public int secs_retry_limit
        {
            get { return c_secs_retry_limit; }
            set { c_secs_retry_limit = value; }
        }
        public Boolean secs_is_multi_block
        {
            get { return c_secs_is_multi_block; }
            set { c_secs_is_multi_block = value; }
        }
        public Boolean secs_rcv_double_block
        {
            get { return c_secs_rcv_double_block; }
            set { c_secs_rcv_double_block = value; }
        }
        public int rs232_baudrate
        {
            get { return c_rs232_baudrate; }
            set { c_rs232_baudrate = value; }
        }
        public int rs232_databits
        {
            get { return c_rs232_databits; }
            set { c_rs232_databits = value; }
        }
        public int rs232_paritybit
        {
            get { return c_rs232_paritybit; }
            set { c_rs232_paritybit = value; }
        }
        public int rs232_handshake
        {
            get { return c_rs232_handshake; }
            set { c_rs232_handshake = value; }
        }
        //public int rs232_rts_enable
        //{
        //    get { return c_rs232_rts_enable; }
        //    set { c_rs232_rts_enable = value; }
        //}
        public int rs232_stop_bits
        {
            get { return c_rs232_stop_bits; }
            set { c_rs232_stop_bits = value; }
        }

        public int handler_type
        {
            get { return c_handler_type; }
            set { c_handler_type = value; }
        }
        public List<CEID_RPTIDS> CEIDS
        {
            get { return c_CEIDS; }
            set { c_CEIDS = value; }
        }

        public void Set_CEIDS(DataTable tmp)
        {
            DB_GetConverter DBCONVERT = new DB_GetConverter();
            List<CEID_RPTIDS> list = new List<CEID_RPTIDS>();
            foreach(DataRow row in tmp.Rows) 
            {
                CEID_RPTIDS Params = new CEID_RPTIDS();
                Params.comm_id = DBCONVERT.GetInt32(row, "comm_id");
                Params.ceid = DBCONVERT.GetInt32(row, "ceid");
                Params.sort_num = DBCONVERT.GetInt32(row, "sort_num");
                Params.rptid = DBCONVERT.GetInt32(row, "rptid");
                Params.description = DBCONVERT.GetString(row, "descriptions");
                list.Add(Params);
            }
            c_CEIDS = list;
        }
        public List<RPTID_VIDS> RPTIDS
        {
            get { return c_RPTIDS; }
            set { c_RPTIDS = value; }
        }
        public void Set_RPTIDS(DataTable tmp)
        {
            DB_GetConverter DBCONVERT = new DB_GetConverter();
            List<RPTID_VIDS> list = new List<RPTID_VIDS>();
            foreach(DataRow row in tmp.Rows) 
            {
                RPTID_VIDS Params = new RPTID_VIDS();
                Params.comm_id = DBCONVERT.GetInt32(row, "comm_id");
                Params.rptid = DBCONVERT.GetInt32(row, "rptid");
                Params.sort_num = DBCONVERT.GetInt32(row, "sort_num");
                Params.vid = DBCONVERT.GetInt32(row, "vid");
                Params.description = DBCONVERT.GetString(row, "descriptions");
                list.Add(Params);
            }
            c_RPTIDS = list;
        }

        public List<Comm_Settings> Comms
        {
            get { return c_Comms; }
            set { c_Comms = value; }
        }
        public void Set_Comms(DataTable tmp)
        {
            DB_GetConverter DBCONVERT = new DB_GetConverter();
            List<Comm_Settings> list = new List<Comm_Settings>();
            foreach (DataRow row in tmp.Rows)
            {
                Comm_Settings Params = new Comm_Settings();
                Params.machine_id = DBCONVERT.GetInt32(row, "machine_id");
                Params.comm_id = DBCONVERT.GetByte(row, "comm_id");
                Params.rs232_com_no = DBCONVERT.GetInt32(row, "rs232c_com_no");
                Params.local_port_no = DBCONVERT.GetInt32(row, "local_port_no");
                Params.secs_device_id = DBCONVERT.GetInt32(row, "secs_device_id");
                Params.recmote_ip = DBCONVERT.GetString(row, "remote_ip");
                Params.remote_port_no = DBCONVERT.GetInt32(row, "remote_port");
                list.Add(Params);
            }
            c_Comms = list;
        }
    }

    public class CEID_RPTIDS
    {
        private int c_comm_id;
        private int c_ceid;
        private int c_sort_num;
        private int c_rptid;
        private String c_description;

        public CEID_RPTIDS()
        {
            c_comm_id = -1;
            c_ceid = -1;
            c_sort_num = -1;
            c_rptid = -1;
            c_description = "";
        }

        public int comm_id
        {
            get { return c_comm_id; }
            set { c_comm_id = value; }
        }
        public int ceid
        {
            get { return c_ceid; }
            set { c_ceid = value; }
        }
        public int sort_num
        {
            get { return c_sort_num; }
            set { c_sort_num = value; }
        }
        public int rptid
        {
            get { return c_rptid; }
            set { c_rptid = value; }
        }
        public String description
        {
            get { return c_description; }
            set { c_description = value; }
        }
    }

    public class RPTID_VIDS
    {
        private int c_comm_id;
        private int c_rptid;
        private int c_sort_num;
        private int c_vid;
        private String c_description;

        public RPTID_VIDS()
        {
            c_comm_id = -1;
            c_rptid = -1;
            c_vid = -1;
            c_description = "";
        }

        public int comm_id
        {
            get { return c_comm_id; }
            set { c_comm_id = value; }
        }
        public int rptid
        {
            get { return c_rptid; }
            set { c_rptid = value; }
        }
        public int sort_num
        {
            get { return c_sort_num; }
            set { c_sort_num = value; }
        }
        public int vid
        {
            get { return c_vid; }
            set { c_vid = value; }
        }
        public String description
        {
            get { return c_description; }
            set { c_description = value; }
        }
    }

    public class Comm_Settings
    {
        private int c_machine_id;
        private int c_comm_id;
        private int c_rs232_com_no;
        private int c_local_port_no;
        private int c_secs_device_id;
        private int c_remote_port_no;
        private String c_recmote_ip;


        public Comm_Settings()
        {
            c_machine_id = -1;
            c_comm_id = -1;
            c_rs232_com_no = -1;
            c_local_port_no = -1;
            c_secs_device_id = -1;
            c_remote_port_no = -1;
            c_recmote_ip = "";
        }

        public int machine_id
        {
            get { return c_machine_id; }
            set { c_machine_id = value; }
        }
        public int comm_id
        {
            get { return c_comm_id; }
            set { c_comm_id = value; }
        }
        public int rs232_com_no
        {
            get { return c_rs232_com_no; }
            set { c_rs232_com_no = value; }
        }
        public int local_port_no
        {
            get { return local_port_no; }
            set { local_port_no = value; }
        }
        public int secs_device_id
        {
            get { return c_secs_device_id; }
            set { c_secs_device_id = value; }
        }
        public int remote_port_no
        {
            get { return c_remote_port_no; }
            set { c_remote_port_no = value; }
        }
        public String recmote_ip
        {
            get { return c_recmote_ip; }
            set { c_recmote_ip = value; }
        }
    }
    #endregion
}
