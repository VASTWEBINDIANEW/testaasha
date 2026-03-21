/*get Loan dropdown text function */
function Loandropdown(x) {
   
    if (x === "AEON Credit") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-1");
        $("#divpayamountloan").removeClass("col-md-2");
        document.getElementById("divLoanmobileno").style.display = "block";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "block";
    }
    if (x === "Aavas Financiers") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Achiievers Gold Loan") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "block";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-1");
        $("#divpayamountloan").removeClass("col-md-2");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("btngetloanviewbill").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        $("#loanpaymenddl").empty();
        $('#loanpaymenddl').append('<option value="Interest Amount">Interest Amount</option>'
        , '<option value="Loan Closure">Loan Closure</option>',
         '<option value="Principle Amount">Principle Amount</option>');
    }
    else if (x === "Aditya Birla Capital") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-1");
        $("#divpayamountloan").removeClass("col-md-2");
        document.getElementById("divLoanmobileno").style.display = "block";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Aspire Home Finance Corporation Limited") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Bajaj Auto Finance") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "block";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-1");
        $("#divpayamountloan").removeClass("col-md-2");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("btngetloanviewbill").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        $("#loanpaymenddl").empty();
        $('#loanpaymenddl').append('<option value="CustomerID">CustomerID</option>'
            , '<option value="Loan Account Number">Loan Account Number</option>',
             '<option value="Mobile Number">Mobile Number</option>',
             '<option value="Provisional RC No">Provisional RC No</option>',
             '<option value="Registration Number">Registration Number</option>');
    }
    else if (x === "Bajaj Finserv") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number ID";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
        }
    else if (x === "Bussan Auto Finance") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "block";
        document.getElementById("txtLoanAccountno").placeholder = "Agreement Number";
        $("#divpayamountloan").addClass("col-md-1");
        $("#divpayamountloan").removeClass("col-md-2");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("btngetloanviewbill").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        $("#loanpaymenddl").empty();
        $('#loanpaymenddl').append('<option value="2 Wheeler">2 Wheeler</option>'
            , '<option value="3 Wheeler">3 Wheeler</option>',
             '<option value="Agriculture & 4 Wheeler">Agriculture & 4 Wheeler</option>',
             '<option value="Personal Loan">Personal Loan</option>');
        }
    else if (x === "Capital First Limited") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Capital Float") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Contract/Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Dewan Housing Finance Corporation Limited") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "ESS KAY FINCORP") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Fintree Finance") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "FlexSalary") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Fullerton India") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }

    else if (x === "HDB Financial Services") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "HDFC Home Loan") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Hero FinCorp") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Application Id";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Home Credit") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Contract/Loan Account Number";
        $("#divpayamountloan").addClass("col-md-1");
        $("#divpayamountloan").removeClass("col-md-2");
        document.getElementById("divLoanmobileno").style.display = "block";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "block";
    }
    else if (x === "INDIABULLS HOUSING FINANCE") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "block";
        document.getElementById("divLoanpancard").style.display = "block";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-1");
        $("#divpayamountloan").removeClass("col-md-2");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Indiabulls Consumer Finance Limited") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Agreement Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Kissht") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "block";
        document.getElementById("divLoanAcno").style.display = "none";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Kosamattam Finance") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "AWB Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "LenDenClub") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan ID(Upper Case Only Ex:-ABC123AC)";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "LoanTAP") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "MAS Financial Services Limited") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "MAS Rural Housing & Mortgage Finance Ltd") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Magma Fincorp Ltd") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Proposal Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Manappuram") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "block";
        document.getElementById("txtLoanpancard").placeholder = "Pledge Value";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Pledge Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("btngetloanviewbill").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        $("#loanpaymenddl").empty();
        $('#loanpaymenddl').append('<option value="Full Payment">Full Payment</option>'
            , '<option value="Interest Payment">Interest Payment</option>',
             '<option value="Partial Payment">Partial Payment</option>');
        divLoanpancard
    }
    else if (x === "Muthoot Fincorp Ltd.") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Reference Code";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Neev Finance") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "block";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        document.getElementById("btngetloanviewbill").style.display = "none";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "block";
        document.getElementById("divLoanAcno").style.display = "none";
        $("#loanpaymenddl").empty();
        $('#loanpaymenddl').append('<option value="Loan EMI Payment">Loan EMI Payment</option>');
    }
    else if (x === "PaisaDukan") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "block";
        document.getElementById("txtLoanAccountno").placeholder = "Loan ID";
        $("#divpayamountloan").addClass("col-md-1");
        $("#divpayamountloan").removeClass("col-md-2");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("btngetloanviewbill").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        $("#loanpaymenddl").empty();
        $('#loanpaymenddl').append('<option value="Borrower EMI">Borrower EMI</option>',
            '<option value="Lender Deposit">Lender Deposit</option>');
    }
    else if (x === "RBL Bank Ltd") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Shriram Housing Finance") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Shriram Transport Finance") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Small Business Fincredit India") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Prospect ID";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Toyota Financial Services") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "block";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-1");
        $("#divpayamountloan").removeClass("col-md-2");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("btngetloanviewbill").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        $("#loanpaymenddl").empty();
        $('#loanpaymenddl').append('<option value="EMI Payment">EMI Payment</option>',
            '<option value="Foreclosure">Foreclosure</option>',
            '<option value="Other">Other</option>');
    }
    else if (x === "Tribe3") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Ujjivan Small Finance Bank") {
        //document.getElementById("divLoanCustomerid").style.display = "block"; 
        document.getElementById("txtLoanName").style.display = "block";
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-1");
        $("#divpayamountloan").removeClass("col-md-2");
        $("#divloanddl").addClass("col-md-2");
        $("#divloanddl").removeClass("col-md-3");
        $("#divLoanAcno").addClass("col-md-2");
        $("#divLoanAcno").removeClass("col-md-3");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none"; // on this
        document.getElementById("divLoanAcno").style.display = "block";
        //document.getElementById("divLoanName").style.display = "block";
        //document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "Volkswagen Finance") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account Number";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
    else if (x === "WheelsEMI") {
        document.getElementById("divLoanCustomerid").style.display = "none";
        document.getElementById("divLoanName").style.display = "none";
        $("#divloanddl").addClass("col-md-3");
        $("#divloanddl").removeClass("col-md-2");
        $("#divLoanAcno").addClass("col-md-3");
        $("#divLoanAcno").removeClass("col-md-2");
        $("#divLoanmobileno").addClass("col-md-2");
        $("#divLoanmobileno").removeClass("col-md-1");
        document.getElementById("divLoandob").style.display = "none";
        document.getElementById("divLoanpancard").style.display = "none";
        document.getElementById("divLoanPaymentMode").style.display = "none";
        document.getElementById("txtLoanAccountno").placeholder = "Loan Account No/Mobile No.";
        $("#divpayamountloan").addClass("col-md-2");
        $("#divpayamountloan").removeClass("col-md-1");
        document.getElementById("divLoanmobileno").style.display = "none";
        document.getElementById("divLoanAcno").style.display = "block";
        document.getElementById("btngetloanviewbill").style.display = "none";
    }
}

/* continue button validation */
function continuebtnvalidationloan(loanddl, txtacno, mobile, Amount, paymodeLoanddl, customerid, Name, date, pancard) {
    if (loanddl === "Select Lender") {
        document.getElementById("Loanoperator").innerHTML = "Please Select Any Lender";
        return false;
    }
    else if (Amount === "") {
        if (loanddl !== "Select Lender") {
            document.getElementById("Loanoperator").innerHTML = "";
        }
        document.getElementById("amountLoanMessage").innerHTML = "Please Enter Amount";
        return false;
    }
    else {
         document.getElementById("Loanoperator").innerHTML = "";
       
        if (txtacno === "" && loanddl === "AEON Credit") {
            document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
            document.getElementById("Loanmessage").style.display = "block";
            document.getElementById("amountLoanMessage").style.display = "none";
            document.getElementById("divpayamountloan").style.marginTop = "8px";
            document.getElementById("divLoanAcno").style.marginTop = "8px"; 
            event.preventDefault();
            return false;
        }

       else if (txtacno == "" && loanddl === "Aavas Financiers") {
            document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
            document.getElementById("Loanmessage").style.display = "block";
            document.getElementById("amountLoanMessage").style.display = "none";
            document.getElementById("divpayamountloan").style.marginTop = "8px";
            document.getElementById("divLoanAcno").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }

       else if (txtacno === "" && loanddl === "Achiievers Gold Loan") {
            document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
            document.getElementById("Loanmessage").style.display = "block";
            document.getElementById("amountLoanMessage").style.display = "none";
            document.getElementById("divpayamountloan").style.marginTop = "8px";
            document.getElementById("divLoanAcno").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }     
  
       else if (txtacno === "" && loanddl === "Aditya Birla Capital") {
            document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
            document.getElementById("Loanmessage").style.display = "block";
            document.getElementById("amountLoanMessage").style.display = "none";
            document.getElementById("divpayamountloan").style.marginTop = "8px";
            document.getElementById("divLoanAcno").style.marginTop = "8px";
            event.preventDefault();
            return false;
        }
    
       else if (mobile === "" && loanddl === "Aditya Birla Capital") {       
           document.getElementById("Loanmobilemessage").innerHTML = "Enter Mobile number";
           document.getElementById("Loanmobilemessage").style.display = "block";
           document.getElementById("Loanmessage").style.display = "none";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }
       else if (mobile !== "" && loanddl === "Aditya Birla Capital") {
           var loanmobile = document.getElementById("txtLoanMobileno").value.length;
           if (loanmobile !== 10) {
               document.getElementById("Loanmobilemessage").innerHTML = "Number should be [10] digits";
               document.getElementById("Loanmobilemessage").style.display = "block";
               document.getElementById("Loanmessage").style.display = "none";
               document.getElementById("amountLoanMessage").style.display = "none";
               document.getElementById("divpayamountloan").style.marginTop = "8px";
               document.getElementById("divLoanAcno").style.marginTop = "8px";
               event.preventDefault();
               return false;
           }
           else {
               document.getElementById("Loanmobilemessage").style.display = "none";
               document.getElementById("Loanmessage").style.display = "none";
               document.getElementById("amountLoanMessage").style.display = "none";
               document.getElementById("divpayamountloan").style.marginTop = "8px";
               document.getElementById("divLoanAcno").style.marginTop = "8px";
               //$('#myViewpaybillmodel').modal('show');
               //   event.preventDefault();
               return true;
           }
       }

       else if (txtacno === "" && loanddl === "Aspire Home Finance Corporation Limited") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Bajaj Auto Finance") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }


       else if (txtacno === "" && loanddl === "BajajFinserv") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (mobile === "" && loanddl === "BajajFinserv") {
           document.getElementById("Loanmobilemessage").innerHTML = "Enter Mobile number";
           document.getElementById("Loanmobilemessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           document.getElementById("Loanmessage").style.display = "none";
           event.preventDefault();
           return false;
       }

       else if (mobile !== "" && loanddl === "BajajFinserv") {
           var loanmobile = document.getElementById("txtLoanMobileno").value.length;
           if (loanmobile !== 10) {
               document.getElementById("Loanmobilemessage").innerHTML = "Number should be [10] digits";
               document.getElementById("Loanmobilemessage").style.display = "block";
               document.getElementById("Loanmessage").style.display = "none";
               document.getElementById("amountLoanMessage").style.display = "none";
               document.getElementById("divpayamountloan").style.marginTop = "8px";
               document.getElementById("divLoanAcno").style.marginTop = "8px";
               event.preventDefault();
               return false;
           }
           else {
               document.getElementById("Loanmobilemessage").style.display = "none";
               document.getElementById("Loanmessage").style.display = "none";
               document.getElementById("amountLoanMessage").style.display = "none";
               document.getElementById("divpayamountloan").style.marginTop = "8px";
               document.getElementById("divLoanAcno").style.marginTop = "8px";
               //$('#myViewpaybillmodel').modal('show');
               //   event.preventDefault();
               return true;
           }
       }

       else if (txtacno === "" && loanddl === "Bussan Auto Finance") {
           document.getElementById("Loanmessage").innerHTML = "Enter Agreement number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Capital First Limited") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Capital Float") {
           document.getElementById("Loanmessage").innerHTML = "Enter Contract/Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Dewan Housing Finance Corporation Limited") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "ESS KAY FINCORP") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Fintree Finance") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "FlexSalary") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Fullerton India") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "HDB Financial Services") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "HDFC Home Loan") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Hero FinCorp") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Home Credit") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (mobile === "" && loanddl === "Home Credit") {
           document.getElementById("Loanmobilemessage").innerHTML = "Enter Mobile number";
           document.getElementById("Loanmobilemessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           document.getElementById("Loanmessage").style.display = "none";
           event.preventDefault();
           return false;
       }

       else if (mobile !== "" && loanddl === "Home Credit") {
           var loanmobile = document.getElementById("txtLoanMobileno").value.length;
           if (loanmobile !== 10) {
               document.getElementById("Loanmobilemessage").innerHTML = "Number should be [10] digits";
               document.getElementById("Loanmobilemessage").style.display = "block";
               document.getElementById("Loanmessage").style.display = "none";
               document.getElementById("amountLoanMessage").style.display = "none";
               document.getElementById("divpayamountloan").style.marginTop = "8px";
               document.getElementById("divLoanAcno").style.marginTop = "8px";
               event.preventDefault();
               return false;
           }
           else {
               document.getElementById("Loanmobilemessage").style.display = "none";
               document.getElementById("Loanmessage").style.display = "none";
               document.getElementById("amountLoanMessage").style.display = "none";
               document.getElementById("divpayamountloan").style.marginTop = "8px";
               document.getElementById("divLoanAcno").style.marginTop = "8px";
               //$('#myViewpaybillmodel').modal('show');
               //   event.preventDefault();
               return true;
           }
       }

       else if (txtacno === "" && loanddl === "INDIABULLS HOUSING FINANCE") {
           document.getElementById("Loanmessage").innerHTML = "Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (date === "" && loanddl === "INDIABULLS HOUSING FINANCE") {
           document.getElementById("Loandobmessage").innerHTML = "Enter DOB";
           document.getElementById("Loandobmessage").style.display = "block";
           document.getElementById("Loanmessage").style.display = "none";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (pancard === "" && loanddl === "INDIABULLS HOUSING FINANCE") {
           document.getElementById("Loanpancardmessage").innerHTML = "Enter Pancard No";
           document.getElementById("Loanpancardmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("Loandobmessage").style.display = "none";
           document.getElementById("Loanmessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Indiabulls Consumer Finance Limited") {
           document.getElementById("Loanmessage").innerHTML = "Enter Loan Agreement number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (mobile === "" && loanddl === "Kissht") {
           document.getElementById("Loanmobilemessage").innerHTML = "Enter Mobile number";
           document.getElementById("Loanmobilemessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           document.getElementById("Loanmessage").style.display = "none";
           event.preventDefault();
           return false;
       }

       else if (mobile !== "" && loanddl === "Kissht") {
           var loanmobile = document.getElementById("txtLoanMobileno").value.length;
           if (loanmobile !== 10) {
               document.getElementById("Loanmobilemessage").innerHTML = "Number should be [10] digits";
               document.getElementById("Loanmobilemessage").style.display = "block";
               document.getElementById("Loanmessage").style.display = "none";
               document.getElementById("amountLoanMessage").style.display = "none";
               document.getElementById("divpayamountloan").style.marginTop = "8px";
               document.getElementById("divLoanAcno").style.marginTop = "8px";
               event.preventDefault();
               return false;
           }
           else {
               document.getElementById("Loanmobilemessage").style.display = "none";
               document.getElementById("Loanmessage").style.display = "none";
               document.getElementById("amountLoanMessage").style.display = "none";
               document.getElementById("divpayamountloan").style.marginTop = "8px";
               document.getElementById("divLoanAcno").style.marginTop = "8px";
               //$('#myViewpaybillmodel').modal('show');
               //   event.preventDefault();
               return true;
           }
       }

       else if (txtacno === "" && loanddl === "Kosamattam Finance") {
           document.getElementById("Loanmessage").innerHTML = "Enter AWB number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "LenDenClub") {
           document.getElementById("Loanmessage").innerHTML = "Enter Loan ID";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "LoanTAP") {
           document.getElementById("Loanmessage").innerHTML = "Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "MAS Financial Services Limited") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "MAS Rural Housing & Mortgage Finance Ltd") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Magma Fincorp Ltd") {
           document.getElementById("Loanmessage").innerHTML = "Enter Proposal number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Manappuram") {
           document.getElementById("Loanmessage").innerHTML = "Enter Pledge number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (pancard === "" && loanddl === "Manappuram") {
           document.getElementById("Loanpancardmessage").innerHTML = "Enter Pancard No";
           document.getElementById("Loanpancardmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           document.getElementById("Loanmessage").style.display = "none";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Muthoot Fincorp Ltd.") {
           document.getElementById("Loanmessage").innerHTML = "Enter Reference Code";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (mobile === "" && loanddl === "Neev Finance") {
           document.getElementById("Loanmobilemessage").innerHTML = "Enter Mobile number";
           document.getElementById("Loanmobilemessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           document.getElementById("Loanmessage").style.display = "none";
           event.preventDefault();
           return false;
       }

       else if (mobile !== "" && loanddl === "Neev Finance") {
           var loanmobile = document.getElementById("txtLoanMobileno").value.length;
           if (loanmobile !== 10) {
               document.getElementById("Loanmobilemessage").innerHTML = "Number should be [10] digits";
               document.getElementById("Loanmobilemessage").style.display = "block";
               document.getElementById("Loanmessage").style.display = "none";
               document.getElementById("amountLoanMessage").style.display = "none";
               document.getElementById("divpayamountloan").style.marginTop = "8px";
               document.getElementById("divLoanAcno").style.marginTop = "8px";
               event.preventDefault();
               return false;
           }
           else {
               document.getElementById("Loanmobilemessage").style.display = "none";
               document.getElementById("Loanmessage").style.display = "none";
               document.getElementById("amountLoanMessage").style.display = "none";
               document.getElementById("divpayamountloan").style.marginTop = "8px";
               document.getElementById("divLoanAcno").style.marginTop = "8px";
               //$('#myViewpaybillmodel').modal('show');
               //   event.preventDefault();
               return true;
           }
       }

       else if (txtacno === "" && loanddl === "PaisaDukan") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan ID";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "RBL Bank Ltd") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Shriram Housing Finance") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Shriram Transport Finance") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Small Business Fincredit India") {
           document.getElementById("Loanmessage").innerHTML = "Enter Prospect ID";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Toyota Financial Services") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Tribe3") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Ujjivan Small Finance Bank") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
        }

        else if (txtacno != "" && loanddl === "Ujjivan Small Finance Bank") {
            return true;
        }


       //else if (customerid === "" && loanddl === "Ujjivan Small Finance Bank") {
       //    document.getElementById("Loancustomermessage").innerHTML = "Enter Customer no";
       //    document.getElementById("Loancustomermessage").style.display = "block";
       //    document.getElementById("amountLoanMessage").style.display = "none";
       //    document.getElementById("divpayamountloan").style.marginTop = "8px";
       //    document.getElementById("divLoanAcno").style.marginTop = "8px";
       //    document.getElementById("Loanmessage").style.display = "none";
       //    event.preventDefault();
       //    return false;
       //}

       //else if (Name === "" && loanddl === "Ujjivan Small Finance Bank") {
       //    document.getElementById("LoanNamemessage").innerHTML = "Enter Name";
       //    document.getElementById("LoanNamemessage").style.display = "block";
       //    document.getElementById("amountLoanMessage").style.display = "none";
       //    document.getElementById("divpayamountloan").style.marginTop = "8px";
       //    document.getElementById("divLoanAcno").style.marginTop = "8px";
       //    document.getElementById("Loanmessage").style.display = "none";
       //    document.getElementById("Loancustomermessage").style.display = "none";
       //    event.preventDefault();
       //    return false;
       //}

       //else if (mobile === "" && loanddl === "Ujjivan Small Finance Bank") {
       //    document.getElementById("Loanmobilemessage").innerHTML = "Please Enter valid Mobile number";
       //    document.getElementById("Loanmobilemessage").style.display = "block";
       //    document.getElementById("amountLoanMessage").style.display = "none";
       //    document.getElementById("divpayamountloan").style.marginTop = "8px";
       //    document.getElementById("divLoanAcno").style.marginTop = "8px";
       //    document.getElementById("Loanmessage").style.display = "none";
       //    document.getElementById("LoanNamemessage").style.display = "none";
       //    document.getElementById("Loancustomermessage").style.display = "none";
       //    event.preventDefault();
       //    return false;
       //}


       //else if (mobile !== "" && loanddl === "Ujjivan Small Finance Bank") {
       //    var loanmobile = document.getElementById("txtLoanMobileno").value.length;
       //    if (loanmobile !== 10) {
       //        document.getElementById("Loanmobilemessage").innerHTML = "Number should be [10] digits";
       //        document.getElementById("Loanmobilemessage").style.display = "block";
       //        document.getElementById("Loanmessage").style.display = "none";
       //        document.getElementById("amountLoanMessage").style.display = "none";
       //        document.getElementById("divpayamountloan").style.marginTop = "8px";
       //        document.getElementById("divLoanAcno").style.marginTop = "8px";
       //        event.preventDefault();
       //        return false;
       //    }
       //    else {
       //        document.getElementById("Loanmobilemessage").style.display = "none";
       //        document.getElementById("Loanmessage").style.display = "none";
       //        document.getElementById("amountLoanMessage").style.display = "none";
       //        document.getElementById("divpayamountloan").style.marginTop = "8px";
       //        document.getElementById("divLoanAcno").style.marginTop = "8px";
       //        //$('#myViewpaybillmodel').modal('show');
       //        //   event.preventDefault();
       //        return true;
       //    }
       //}

       else if (txtacno === "" && loanddl === "Volkswagen Finance") {
           document.getElementById("Loanmessage").innerHTML = "Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "WheelsEMI") {
           document.getElementById("Loanmessage").innerHTML = "Enter Loan A/c No/Mobile no";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (txtacno === "" && loanddl === "Aditya Birla Housing Finance Limited") {
           document.getElementById("Loanmessage").innerHTML = "Please Enter Loan Account number";
           document.getElementById("Loanmessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           event.preventDefault();
           return false;
       }

       else if (mobile === "" && loanddl === "Aditya Birla Housing Finance Limited") {
           document.getElementById("Loanmobilemessage").innerHTML = "Please Enter valid Mobile Number";
           document.getElementById("Loanmobilemessage").style.display = "block";
           document.getElementById("amountLoanMessage").style.display = "none";
           document.getElementById("divpayamountloan").style.marginTop = "8px";
           document.getElementById("divLoanAcno").style.marginTop = "8px";
           document.getElementById("Loanmessage").style.display = "none";
           event.preventDefault();
           return false;
       }

       else if (mobile !== "" && loanddl === "Aditya Birla Housing Finance Limited") {
           var loanmobile = document.getElementById("txtLoanMobileno").value.length;
           if (loanmobile !== 10) {
               document.getElementById("Loanmobilemessage").innerHTML = "Number should be [10] digits";
               document.getElementById("Loanmobilemessage").style.display = "block";
               document.getElementById("Loanmessage").style.display = "none";
               document.getElementById("amountLoanMessage").style.display = "none";
               document.getElementById("divpayamountloan").style.marginTop = "8px";
               document.getElementById("divLoanAcno").style.marginTop = "8px";
               event.preventDefault();
               return false;
           }
           else {
               document.getElementById("Loanmobilemessage").style.display = "none";
               document.getElementById("Loanmessage").style.display = "none";
               document.getElementById("amountLoanMessage").style.display = "none";
               document.getElementById("divpayamountloan").style.marginTop = "8px";
               document.getElementById("divLoanAcno").style.marginTop = "8px";
               //$('#myViewpaybillmodel').modal('show');
               //   event.preventDefault();
               return true;
           }
       }

       else {
           document.getElementById("Loanmessage").style.display = "none";
           document.getElementById("Loanmobilemessage").style.display = "none";
           document.getElementById("amountLoanMessage").style.display = "none";
           return true;
       }

    }
}

/*Paybillbutton javascript */

