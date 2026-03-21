using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Vastwebmulti.Areas.RCH.Models
{
    public class Manual_rch
    {
    }
    public partial class Select_balance_Super_stokist_Result : ComplexObject
    {
        #region Factory Method

        /// <summary>
        /// Create a new Select_balance_Super_stokist_Result object.
        /// </summary>
        /// <param name="id">Initial value of the ID property.</param>
        /// <param name="superStokistID">Initial value of the SuperStokistID property.</param>
        /// <param name="cr">Initial value of the cr property.</param>
        public static Select_balance_Super_stokist_Result CreateSelect_balance_Super_stokist_Result(global::System.Int32 id, global::System.String superStokistID, global::System.Decimal cr)
        {
            Select_balance_Super_stokist_Result select_balance_Super_stokist_Result = new Select_balance_Super_stokist_Result();
            select_balance_Super_stokist_Result.ID = id;
            select_balance_Super_stokist_Result.SuperStokistID = superStokistID;
            select_balance_Super_stokist_Result.cr = cr;
            return select_balance_Super_stokist_Result;
        }

        #endregion

        #region Primitive Properties

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = false)]
        [DataMemberAttribute()]
        public global::System.Int32 ID
        {
            get
            {
                return _ID;
            }
            set
            {
                OnIDChanging(value);
                ReportPropertyChanging("ID");
                _ID = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ID");
                OnIDChanged();
            }
        }
        private global::System.Int32 _ID;
        partial void OnIDChanging(global::System.Int32 value);
        partial void OnIDChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = false)]
        [DataMemberAttribute()]
        public global::System.String SuperStokistID
        {
            get
            {
                return _SuperStokistID;
            }
            set
            {
                OnSuperStokistIDChanging(value);
                ReportPropertyChanging("SuperStokistID");
                _SuperStokistID = StructuralObject.SetValidValue(value, false);
                ReportPropertyChanged("SuperStokistID");
                OnSuperStokistIDChanged();
            }
        }
        private global::System.String _SuperStokistID;
        partial void OnSuperStokistIDChanging(global::System.String value);
        partial void OnSuperStokistIDChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> Balance
        {
            get
            {
                return _Balance;
            }
            set
            {
                OnBalanceChanging(value);
                ReportPropertyChanging("Balance");
                _Balance = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("Balance");
                OnBalanceChanged();
            }
        }
        private Nullable<global::System.Decimal> _Balance;
        partial void OnBalanceChanging(Nullable<global::System.Decimal> value);
        partial void OnBalanceChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> RechargeDate
        {
            get
            {
                return _RechargeDate;
            }
            set
            {
                OnRechargeDateChanging(value);
                ReportPropertyChanging("RechargeDate");
                _RechargeDate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("RechargeDate");
                OnRechargeDateChanged();
            }
        }
        private Nullable<global::System.DateTime> _RechargeDate;
        partial void OnRechargeDateChanging(Nullable<global::System.DateTime> value);
        partial void OnRechargeDateChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> remainsuper
        {
            get
            {
                return _remainsuper;
            }
            set
            {
                OnremainsuperChanging(value);
                ReportPropertyChanging("remainsuper");
                _remainsuper = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("remainsuper");
                OnremainsuperChanged();
            }
        }
        private Nullable<global::System.Decimal> _remainsuper;
        partial void OnremainsuperChanging(Nullable<global::System.Decimal> value);
        partial void OnremainsuperChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> commistion
        {
            get
            {
                return _commistion;
            }
            set
            {
                OncommistionChanging(value);
                ReportPropertyChanging("commistion");
                _commistion = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("commistion");
                OncommistionChanged();
            }
        }
        private Nullable<global::System.Decimal> _commistion;
        partial void OncommistionChanging(Nullable<global::System.Decimal> value);
        partial void OncommistionChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> remainsuperafter
        {
            get
            {
                return _remainsuperafter;
            }
            set
            {
                OnremainsuperafterChanging(value);
                ReportPropertyChanging("remainsuperafter");
                _remainsuperafter = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("remainsuperafter");
                OnremainsuperafterChanged();
            }
        }
        private Nullable<global::System.Decimal> _remainsuperafter;
        partial void OnremainsuperafterChanging(Nullable<global::System.Decimal> value);
        partial void OnremainsuperafterChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public global::System.String bal_type
        {
            get
            {
                return _bal_type;
            }
            set
            {
                Onbal_typeChanging(value);
                ReportPropertyChanging("bal_type");
                _bal_type = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("bal_type");
                Onbal_typeChanged();
            }
        }
        private global::System.String _bal_type;
        partial void Onbal_typeChanging(global::System.String value);
        partial void Onbal_typeChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> oldcrbalance
        {
            get
            {
                return _oldcrbalance;
            }
            set
            {
                OnoldcrbalanceChanging(value);
                ReportPropertyChanging("oldcrbalance");
                _oldcrbalance = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("oldcrbalance");
                OnoldcrbalanceChanged();
            }
        }
        private Nullable<global::System.Decimal> _oldcrbalance;
        partial void OnoldcrbalanceChanging(Nullable<global::System.Decimal> value);
        partial void OnoldcrbalanceChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public global::System.String SuperstokistName
        {
            get
            {
                return _SuperstokistName;
            }
            set
            {
                OnSuperstokistNameChanging(value);
                ReportPropertyChanging("SuperstokistName");
                _SuperstokistName = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("SuperstokistName");
                OnSuperstokistNameChanged();
            }
        }
        private global::System.String _SuperstokistName;
        partial void OnSuperstokistNameChanging(global::System.String value);
        partial void OnSuperstokistNameChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public global::System.String FarmName
        {
            get
            {
                return _FarmName;
            }
            set
            {
                OnFarmNameChanging(value);
                ReportPropertyChanging("FarmName");
                _FarmName = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("FarmName");
                OnFarmNameChanged();
            }
        }
        private global::System.String _FarmName;
        partial void OnFarmNameChanging(global::System.String value);
        partial void OnFarmNameChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public global::System.String comment
        {
            get
            {
                return _comment;
            }
            set
            {
                OncommentChanging(value);
                ReportPropertyChanging("comment");
                _comment = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("comment");
                OncommentChanged();
            }
        }
        private global::System.String _comment;
        partial void OncommentChanging(global::System.String value);
        partial void OncommentChanged();

        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = false)]
        [DataMemberAttribute()]
        public global::System.Decimal cr
        {
            get
            {
                return _cr;
            }
            set
            {
                OncrChanging(value);
                ReportPropertyChanging("cr");
                _cr = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("cr");
                OncrChanged();
            }
        }
        private global::System.Decimal _cr;
        partial void OncrChanging(global::System.Decimal value);
        partial void OncrChanged();

        #endregion

    }
}