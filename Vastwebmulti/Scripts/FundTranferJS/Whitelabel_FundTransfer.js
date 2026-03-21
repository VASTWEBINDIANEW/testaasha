

$('#btnddbackshow').click(function () {
    $("input:text").val("");
    $('#frmdata1').show();
    $('#hdddninputvalues').empty();
    $('#DIstribotorshowinformation').hide();

})
$('#btnbackshow').click(function () {
    $("input:text").val("");
    $('#frmdata').show();
    $('#hdninputvalues').empty();
    $('#showinformation').hide();
    $('#CallMASTER').hide();

})
$('#btnRRbackshow').click(function () {
    $("input:text").val("");
    $('#formRetailer').show();
    $('#hdRETninputvalues').empty();
    $('#Retailershowinformation').hide();
    $('#CallMASTER').hide();

})

$('#bankid').change(function () {
    var acno = $('#bankid').val();
    $('#txtsupraccno').val(acno)

})
$('#ddlsuprwalletname').change(function () {
    var acno = $('#ddlsuprwalletname').val();
    $('#txtsuprwalletno').val(acno)

})

$('#DISTRIbankid').change(function () {
    var acno = $('#DISTRIbankid').val();
    $('#txtDISTACCOUNTNO').val(acno)
})

$('#ddlDISTwalletname').change(function () {
    var acno = $('#ddlDISTwalletname').val();
    $('#txtDISTWALLETNO').val(acno)

})

$('#RETRIbankid').change(function () {
    var acno = $('#RETRIbankid').val();
    $('#txtRETACCOUNTNO').val(acno)

})
$('#ddlRETwalletname').change(function () {
    var acno = $('#ddlRETwalletname').val();
    $('#txtRETWALLETNO').val(acno)

})

///Retailer Start
$('#txtRETAMOUNT,#txtsupramount,#txtDISTAMOUNT,#txtAPIAMOUNT,#txtWLAMOUNT').keypress(function (e) {
    if (this.value.length == 0 && e.which == 48) {
        return false;
    }
});
$('#btnRetailerInform').click(function () {

    $('#hdRETninputvalues').empty();

    if ($('#RLMID').val() == '' || $('#RLMID').val() == null) {
        $(".errorss").text('Please Select Retailer');
        return false;
    }
    if ($('#ddlRETPaymentModel').val() == '' || $('#ddlRETPaymentModel').val() == null) {
        $(".errorss").text('Pay mode REQUIRED');
        return false;
    }
    if ($('#ddlRETPaymentModel').val() == 1) {
        if ($('#txtRETCollectionBy').val() == '' || $('#txtRETCollectionBy').val() == null) {
            $(".errorss").text('Collection Person Name');
            return false;
        }
        if ($('#txtRETAMOUNT').val() == '' || $('#txtRETAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtRETCOMMENTS').val() == '' || $('#txtRETCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }


    }
    else if ($('#ddlRETPaymentModel').val() == 2) {

        if ($('#RETRIbankid').val() == '' || $('#RETRIbankid').val() == null) {
            $(".errorss").text('Select Bank Name');
            return false;
        }
        if ($('#txtRETAMOUNT').val() == '' || $('#txtRETAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtRETDEPOSITESLIPNO').val() == '' || $('#txtRETDEPOSITESLIPNO').val() == null) {
            $(".errorss").text('Enter Deposite Slip No.');
            return false;
        }

        if ($('#txtRETCOMMENTS').val() == '' || $('#txtRETCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }

    }
    else if ($('#ddlRETPaymentModel').val() == 3) {

        if ($('#RETTransferType').val() == '' || $('#RETTransferType').val() == null) {
            $(".errorss").text('Select Transfer Type');
            return false;
        }
        if ($('#RETRIbankid').val() == '' || $('#RETRIbankid').val() == null) {
            $(".errorss").text('Select Bank Name');
            return false;
        }
        if ($('#txtRETACCOUNTNO').val() == '' || $('#txtRETACCOUNTNO').val() == null) {
            $(".errorss").text('Account No Required');
            return false;
        }
        if ($('#txtRETUTRNO').val() == '' || $('#txtRETUTRNO').val() == null) {
            $(".errorss").text('UTR No Required');
            return false;
        }
        if ($('#txtRETAMOUNT').val() == '' || $('#txtRETAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }

    }
    else if ($('#ddlRETPaymentModel').val() == 4) {

        if ($('#ddlRETwalletname').val() == '' || $('#ddlRETwalletname').val() == null) {
            $(".errorss").text('Collection Person Name');
            return false;
        }
        if ($('#txtRETWALLETNO').val() == '' || $('#txtRETWALLETNO').val() == null) {
            $(".errorss").text('Wallet No. Required');
            return false;
        }
        if ($('#txtRETTranstionNO').val() == '' || $('#txtRETTranstionNO').val() == null) {
            $(".errorss").text('Transtion No.');
            return false;
        }
        if ($('#txtRETAMOUNT').val() == '' || $('#txtRETAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtRETCOMMENTS').val() == '' || $('#txtRETCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }
    else if ($('#ddlRETPaymentModel').val() == 5) {

        if ($('#txtRETAMOUNT').val() == '' || $('#txtRETAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtRETCOMMENTS').val() == '' || $('#txtRETCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }

    }
    else if ($('#ddlRETPaymentModel').val() == 6) {

        if ($('#txtRETSETTLEMENTTYPE').val() == '' || $('#txtRETSETTLEMENTTYPE').val() == null) {
            $(".errorss").text('Settlement Type');
            return false;
        }
        if ($('#txtRETAMOUNT').val() == '' || $('#txtRETAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtRETCOMMENTS').val() == '' || $('#txtRETCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }
    else if ($('#ddlRETPaymentModel').val() == 7) {

        if ($('#txtRETCREDITDETAILS').val() == '' || $('#txtRETCREDITDETAILS').val() == null) {
            $(".errorss").text('Credit Detail Required');
            return false;
        }
        if ($('#txtRETAMOUNT').val() == '' || $('#txtRETAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtRETCOMMENTS').val() == '' || $('#txtRETCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }
    else if ($('#ddlRETPaymentModel').val() == 8) {

        if ($('#txtRETREASON').val() == '' || $('#txtRETREASON').val() == null) {
            $(".errorss").text('Enter Debit Reason');
            return false;
        }
        if ($('#txtRETAMOUNT').val() == '' || $('#txtRETAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtRETCOMMENTS').val() == '' || $('#txtRETCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }
    else if ($('#ddlRETPaymentModel').val() == 9) {

        if ($('#txtRETREASON').val() == '' || $('#txtRETREASON').val() == null) {
            $(".errorss").text('Enter Debit Reason');
            return false;
        }
        if ($('#txtRETAMOUNT').val() == '' || $('#txtRETAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtRETCOMMENTS').val() == '' || $('#txtRETCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }


    var name = $("#RLMID option:selected");
    var idst = $("#ddlRETPaymentModel option:selected").text();
    var collper = $('#txtRETCollectionBy').val();
    var amount = $('#txtRETAMOUNT').val();

    var comment = $('#txtRETCOMMENTS').val();
    var bankname = $("#RETRIbankid option:selected").text();
    var depositeslip = $('#txtRETDEPOSITESLIPNO').val();
    var tranftype = $("#RETTransferType option:selected");
    var accountno = $('#txtRETACCOUNTNO').val();
    var UTRnos = $('#txtRETUTRNO').val();
    var walletname = $("#ddlRETwalletname option:selected");
    var walletno = $('#txtRETWALLETNO').val();
    var transectionno = $('#txtRETTranstionNO').val();
    var setteltype = $('#txtRETSETTLEMENTTYPE').val();
    var crdetails = $('#txtRETCREDITDETAILS').val();
    var sbject = $('#txtRETREASON').val();
    var accnosele = $('#RETRIbankid').val()
    $('#d-name').text(name.text())

    $('#Retailershowinformation').show();

    var thtm = '<div id="hdidsss">';
    thtm += '<input id="hdSuperstokistID" name="hdSuperstokistID" type="hidden" name="classid" value="' + $('#RLMID').val() + '" />';
    thtm += '<input id="hdPaymentMode" name="hdPaymentMode" type="hidden"  value="' + idst + '" />';
    
    $('#RR-collection').parent().hide();
    $('#RR-ammount').parent().hide();
    $('#RR-comment').parent().hide();
    $('#RR-Bank').parent().hide();
    $('#RR-ammount').parent().hide();
    $('#RR-DepositeSlipNo').parent().hide();
    $('#RR-comment').parent().hide();
    $('#RR-Bank').parent().hide();
    $('#RR-ammount').parent().hide();
    $('#RR-DepositeSlipNo').parent().hide();
    $('#RR-comment').parent().hide();
    $('#RR-utr-no').parent().hide();
    $('#RR-subject').parent().hide();
    $('#RR-comment').parent().hide();
    $('#RR-subject').parent().hide();
    $('#RR-account-no').parent().hide();
    $('#RR-TransferType').parent().hide();
    $('#RR-wallet').parent().hide();
    $('#RR-wallet-no').parent().hide();
    $('#RR-transation-no').parent().hide();
    $('#RR-CreditDetail').parent().hide();
    $('#RR-settelment').parent().hide();
    $('#RR-wallet-no').parent().hide();
    $('#RR-name').text(name.text())
    $('#RR-Mode').text(idst)

    if ($("#ddlRETPaymentModel option:selected").index() === 0) {
        $('#RR-collection').parent().show();
        $('#RR-ammount').parent().show();
        $('#RR-comment').parent().show();

        $('#RR-collection').text(collper)
        $('#RR-ammount').text(amount)
        $('#RR-comment').text(comment)
        thtm += '<input id="hdMDcollection" name="hdMDcollection" type="hidden"  value="' + collper + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#ddlRETPaymentModel option:selected").index() === 1) {
        $('#RR-Bank').parent().show();
        $('#RR-ammount').parent().show();
        $('#RR-DepositeSlipNo').parent().show();
        $('#RR-comment').parent().show();


        $('#RR-Bank').text(bankname)
        $('#RR-ammount').text(amount)
        $('#RR-DepositeSlipNo').text(depositeslip);
        $('#RR-comment').text(comment)

        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accnosele + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDDepositeSlipNo" name="hdMDDepositeSlipNo" type="hidden"  value="' + depositeslip + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#ddlRETPaymentModel option:selected").index() === 2) {
        $('#RR-TransferType').parent().show();
        $('#RR-Bank').parent().show();
        $('#RR-ammount').parent().show();
        $('#RR-account-no').parent().show();
        $('#RR-utr-no').parent().show();

        $('#RR-TransferType').text(tranftype.text());
        $('#RR-Bank').text(bankname)
        $('#RR-account-no').text(accountno);
        $('#RR-utr-no').text(UTRnos)
        $('#RR-ammount').text(amount)
        thtm += '<input id="hdMDTransferType" name="hdMDTransferType" type="hidden"  value="' + tranftype.text() + '" />';
        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accountno + '" />';
        thtm += '<input id="hdMDutrno" name="hdMDutrno" type="hidden"  value="' + UTRnos + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#ddlRETPaymentModel option:selected").index() === 3) {
        $('#RR-wallet').parent().show();
        $('#RR-wallet-no').parent().show();
        $('#RR-transation-no').parent().show();
        $('#RR-ammount').parent().show();
        $('#RR-comment').parent().show();

        $('#RR-wallet').text(walletname.text())
        $('#RR-wallet-no').text(walletno)
        $('#RR-transation-no').text(transectionno)
        $('#RR-ammount').text(amount)
        $('#RR-comment').text(comment)
        thtm += '<input id="hdMDwallet" name="hdMDwallet" type="hidden"  value="' + walletname.text() + '" />';
        thtm += '<input id="hdMDwalletno" name="hdMDwalletno" type="hidden"  value="' + walletno + '" />';
        thtm += '<input id="hdMDtransationno" name="hdMDtransationno" type="hidden"  value="' + transectionno + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#ddlRETPaymentModel option:selected").index() === 4) {
        $('#RR-ammount').parent().show();
        $('#RR-comment').parent().show();

        $('#RR-ammount').text(amount)
        $('#RR-comment').text(comment)


        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#ddlRETPaymentModel option:selected").index() === 5) {
        $('#RR-settelment').parent().show();
        $('#RR-ammount').parent().show();
        $('#RR-comment').parent().show();

        $('#RR-settelment').text(setteltype);
        $('#RR-ammount').text(amount)
        $('#RR-comment').text(comment)
        thtm += '<input id="hdMDsettelment" name="hdMDsettelment" type="hidden"  value="' + setteltype + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#ddlRETPaymentModel option:selected").index() === 6) {
        $('#RR-CreditDetail').parent().show();
        $('#RR-ammount').parent().show();
        $('#RR-comment').parent().show();



        $('#RR-CreditDetail').text(crdetails)
        $('#RR-ammount').text(amount)
        $('#RR-comment').text(comment)

        thtm += '<input id="hdMDCreditDetail" name="hdMDCreditDetail" type="hidden"  value="' + crdetails + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';

    }
    if ($("#ddlRETPaymentModel option:selected").index() === 7 || $("#ddlRETPaymentModel option:selected").index() === 8) {
        $('#RR-subject').parent().show();
        $('#RR-ammount').parent().show();
        $('#RR-comment').parent().show();


        $('#RR-subject').text(sbject)
        $('#RR-ammount').text(amount)
        $('#RR-comment').text(comment)
        thtm += '<input id="hdMDsubject" name="hdMDsubject" type="hidden"  value="' + sbject + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    thtm += '</div>';
    $('#formRetailer').hide();
    $('.txtcodecodec').val('');

    $('#hdRETninputvalues').html(thtm);
})
///Retailer END

///Distreibutort Start
$('#btndistributorInform').click(function () {

    $('#hdddninputvalues').empty();

    if ($('#ddlDistributor').val() == '' || $('#ddlDistributor').val() == null) {
        $(".errorss").text('Select Distributor');
        return false;
    }
    if ($('#ddlDISTPaymentModel').val() == '' || $('#ddlDISTPaymentModel').val() == null) {
        $(".errorss").text('Pay mode REQUIRED');
        return false;
    }
    if ($('#ddlDISTPaymentModel').val() == 1) {
        if ($('#txtDISTCollectionBy').val() == '' || $('#txtDISTCollectionBy').val() == null) {
            $(".errorss").text('Collection Person Name');
            return false;
        }
        if ($('#txtDISTAMOUNT').val() == '' || $('#txtDISTAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtDISTCOMMENTS').val() == '' || $('#txtDISTCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }


    }
    else if ($('#ddlDISTPaymentModel').val() == 2) {

        if ($('#DISTRIbankid').val() == '' || $('#DISTRIbankid').val() == null) {
            $(".errorss").text('Select Bank Name');
            return false;
        }
        if ($('#txtDISTAMOUNT').val() == '' || $('#txtDISTAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtDISTDEPOSITESLIPNO').val() == '' || $('#txtDISTDEPOSITESLIPNO').val() == null) {
            $(".errorss").text('Enter Deposite Slip No.');
            return false;
        }

        if ($('#txtDISTCOMMENTS').val() == '' || $('#txtDISTCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }

    }

    else if ($('#ddlDISTPaymentModel').val() == 3) {

        if ($('#DDTransferType').val() == '' || $('#DDTransferType').val() == null) {
            $(".errorss").text('Select Transfer Type');
            return false;
        }
        if ($('#DISTRIbankid').val() == '' || $('#DISTRIbankid').val() == null) {
            $(".errorss").text('Select Bank Name');
            return false;
        }
        if ($('#txtDISTACCOUNTNO').val() == '' || $('#txtDISTACCOUNTNO').val() == null) {
            $(".errorss").text('Account No Required');
            return false;
        }
        if ($('#txtDISTUTRNO').val() == '' || $('#txtDISTUTRNO').val() == null) {
            $(".errorss").text('UTR No Required');
            return false;
        }
        if ($('#txtDISTAMOUNT').val() == '' || $('#txtDISTAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }

    }
    else if ($('#ddlDISTPaymentModel').val() == 4) {

        if ($('#ddlDISTwalletname').val() == '' || $('#ddlDISTwalletname').val() == null) {
            $(".errorss").text('Collection Person Name');
            return false;
        }
        if ($('#txtDISTWALLETNO').val() == '' || $('#txtDISTWALLETNO').val() == null) {
            $(".errorss").text('Wallet No. Required');
            return false;
        }
        if ($('#txtDISTTranstionNO').val() == '' || $('#txtDISTTranstionNO').val() == null) {
            $(".errorss").text('Transtion No.');
            return false;
        }
        if ($('#txtDISTAMOUNT').val() == '' || $('#txtDISTAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtDISTCOMMENTS').val() == '' || $('#txtDISTCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }
    else if ($('#ddlDISTPaymentModel').val() == 5) {

        if ($('#txtDISTAMOUNT').val() == '' || $('#txtDISTAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtDISTCOMMENTS').val() == '' || $('#txtDISTCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }

    }
    else if ($('#ddlDISTPaymentModel').val() == 6) {

        if ($('#txtDISTSETTLEMENTTYPE').val() == '' || $('#txtDISTSETTLEMENTTYPE').val() == null) {
            $(".errorss").text('Settlement Type');
            return false;
        }
        if ($('#txtDISTAMOUNT').val() == '' || $('#txtDISTAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtDISTCOMMENTS').val() == '' || $('#txtDISTCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }
    else if ($('#ddlDISTPaymentModel').val() == 7) {

        if ($('#txtDISTCREDITDETAILS').val() == '' || $('#txtDISTCREDITDETAILS').val() == null) {
            $(".errorss").text('Credit Detail Required');
            return false;
        }
        if ($('#txtDISTAMOUNT').val() == '' || $('#txtDISTAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtDISTCOMMENTS').val() == '' || $('#txtDISTCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }
    else if ($('#ddlDISTPaymentModel').val() == 8) {

        if ($('#txtDISTREASON').val() == '' || $('#txtDISTREASON').val() == null) {
            $(".errorss").text('Enter Debit Reason');
            return false;
        }
        if ($('#txtDISTAMOUNT').val() == '' || $('#txtDISTAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtDISTCOMMENTS').val() == '' || $('#txtDISTCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }
    else if ($('#ddlDISTPaymentModel').val() == 9) {

        if ($('#txtDISTREASON').val() == '' || $('#txtDISTREASON').val() == null) {
            $(".errorss").text('Enter Debit Reason');
            return false;
        }
        if ($('#txtDISTAMOUNT').val() == '' || $('#txtDISTAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtDISTCOMMENTS').val() == '' || $('#txtDISTCOMMENTS').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }

    var name = $("#ddlDistributor option:selected");
    var idst = $("#ddlDISTPaymentModel option:selected").text();
    var collper = $('#txtDISTCollectionBy').val();
    var amount = $('#txtDISTAMOUNT').val();

    var comment = $('#txtDISTCOMMENTS').val();
    var bankname = $("#DISTRIbankid option:selected").text();
    var depositeslip = $('#txtDISTDEPOSITESLIPNO').val();
    var tranftype = $("#ddlsuprtransfertype option:selected");
    var accountno = $('#txtDISTACCOUNTNO').val();
    var UTRnos = $('#txtDISTUTRNO').val();
    var walletname = $("#ddlDISTwalletname option:selected");
    var walletno = $('#txtDISTWALLETNO').val();
    var transectionno = $('#txtDISTTranstionNO').val();
    var setteltype = $('#txtDISTSETTLEMENTTYPE').val();
    var crdetails = $('#txtDISTCREDITDETAILS').val();
    var sbject = $('#txtDISTREASON').val();
    var accnosele = $("#DISTRIbankid").val();
    $('#d-name').text(name.text())

    $('#DIstribotorshowinformation').show();

    var thtm = '<div id="hdidsss">';
    thtm += '<input id="hdSuperstokistID" name="hdSuperstokistID" type="hidden" name="classid" value="' + $('#ddlDistributor').val() + '" />';
    thtm += '<input id="hdPaymentMode" name="hdPaymentMode" type="hidden"  value="' + idst + '" />';
    //  thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' +amount + '" />';
    $('#d-collection').parent().hide();
    $('#d-ammount').parent().hide();
    $('#d-comment').parent().hide();
    $('#d-Bank').parent().hide();
    $('#d-ammount').parent().hide();
    $('#d-DepositeSlipNo').parent().hide();
    $('#d-comment').parent().hide();
    $('#d-Bank').parent().hide();
    $('#d-ammount').parent().hide();
    $('#d-DepositeSlipNo').parent().hide();
    $('#d-comment').parent().hide();
    $('#d-utr-no').parent().hide();
    $('#d-subject').parent().hide();
    $('#d-comment').parent().hide();
    $('#d-subject').parent().hide();
    $('#d-account-no').parent().hide();
    $('#d-TransferType').parent().hide();
    $('#d-wallet').parent().hide();
    $('#d-wallet-no').parent().hide();
    $('#d-transation-no').parent().hide();
    $('#d-CreditDetail').parent().hide();
    $('#d-settelment').parent().hide();
    $('#d-wallet-no').parent().hide();
    $('#d-name').text(name.text())
    $('#d-Mode').text(idst)

    if ($("#ddlDISTPaymentModel option:selected").index() === 0) {
        $('#d-collection').parent().show();
        $('#d-ammount').parent().show();
        $('#d-comment').parent().show();

        $('#d-collection').text(collper)
        $('#d-ammount').text(amount)
        $('#d-comment').text(comment)
        thtm += '<input id="hdMDcollection" name="hdMDcollection" type="hidden"  value="' + collper + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#ddlDISTPaymentModel option:selected").index() === 1) {
        $('#d-Bank').parent().show();
        $('#d-ammount').parent().show();
        $('#d-DepositeSlipNo').parent().show();
        $('#d-comment').parent().show();


        $('#d-Bank').text(bankname)
        $('#d-ammount').text(amount)
        $('#d-DepositeSlipNo').text(depositeslip);
        $('#d-comment').text(comment)

        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accnosele + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDDepositeSlipNo" name="hdMDDepositeSlipNo" type="hidden"  value="' + depositeslip + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#ddlDISTPaymentModel option:selected").index() === 2) {
        $('#d-TransferType').parent().show();
        $('#d-Bank').parent().show();
        $('#d-ammount').parent().show();
        $('#d-account-no').parent().show();
        $('#d-utr-no').parent().show();

        $('#d-TransferType').text(tranftype.text());
        $('#d-Bank').text(bankname)
        $('#d-account-no').text(accountno);
        $('#d-utr-no').text(UTRnos)
        $('#d-ammount').text(amount)
        thtm += '<input id="hdMDTransferType" name="hdMDTransferType" type="hidden"  value="' + tranftype.text() + '" />';
        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accountno + '" />';
        thtm += '<input id="hdMDutrno" name="hdMDutrno" type="hidden"  value="' + UTRnos + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#ddlDISTPaymentModel option:selected").index() === 3) {
        $('#d-wallet').parent().show();
        $('#d-wallet-no').parent().show();
        $('#d-transation-no').parent().show();
        $('#d-ammount').parent().show();
        $('#d-comment').parent().show();

        $('#d-wallet').text(walletname.text())
        $('#d-wallet-no').text(walletno)
        $('#d-transation-no').text(transectionno)
        $('#d-ammount').text(amount)
        $('#d-comment').text(comment)
        thtm += '<input id="hdMDwallet" name="hdMDwallet" type="hidden"  value="' + walletname.text() + '" />';
        thtm += '<input id="hdMDwalletno" name="hdMDwalletno" type="hidden"  value="' + walletno + '" />';
        thtm += '<input id="hdMDtransationno" name="hdMDtransationno" type="hidden"  value="' + transectionno + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#ddlDISTPaymentModel option:selected").index() === 4) {
        $('#d-ammount').parent().show();
        $('#d-comment').parent().show();

        $('#d-ammount').text(amount)
        $('#d-comment').text(comment)


        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#ddlDISTPaymentModel option:selected").index() === 5) {
        $('#d-settelment').parent().show();
        $('#d-ammount').parent().show();
        $('#d-comment').parent().show();

        $('#d-settelment').text(setteltype);
        $('#d-ammount').text(amount)
        $('#d-comment').text(comment)
        thtm += '<input id="hdMDsettelment" name="hdMDsettelment" type="hidden"  value="' + setteltype + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#ddlDISTPaymentModel option:selected").index() === 6) {
        $('#d-CreditDetail').parent().show();
        $('#d-ammount').parent().show();
        $('#d-comment').parent().show();



        $('#d-CreditDetail').text(crdetails)
        $('#d-ammount').text(amount)
        $('#d-comment').text(comment)

        thtm += '<input id="hdMDCreditDetail" name="hdMDCreditDetail" type="hidden"  value="' + crdetails + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';

    }
    if ($("#ddlDISTPaymentModel option:selected").index() === 7 || $("#ddlDISTPaymentModel option:selected").index() === 8) {
        $('#d-subject').parent().show();
        $('#d-ammount').parent().show();
        $('#d-comment').parent().show();


        $('#d-subject').text(sbject)
        $('#d-ammount').text(amount)
        $('#d-comment').text(comment)
        thtm += '<input id="hdMDsubject" name="hdMDsubject" type="hidden"  value="' + sbject + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    thtm += '</div>';
    $('#frmdata1').hide();
    $('.txtcodecodec').val('');
    // $('#txtcode').val('')
    $('#hdddninputvalues').html(thtm);



})
///Distreibutort END

///Super TockList Start
$('#btnshowdetails').click(function () {



    $('#hdninputvalues').empty();

    if ($('#SuperstokistID').val() == '' || $('#SuperstokistID').val() == null) {
        $(".errorss").text('MD LIST REQUIRED');
        return false;
    }
    if ($('#idst').val() == '' || $('#idst').val() == null) {
        $(".errorss").text('Pay mode REQUIRED');
        return false;
    }
    if ($('#idst').val() == 1) {
        if ($('#txtsuprcollperson').val() == '' || $('#txtsuprcollperson').val() == null) {
            $(".errorss").text('Collection Person Name');
            return false;
        }
        if ($('#txtsupramount').val() == '' || $('#txtsupramount').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtsuprarea').val() == '' || $('#txtsuprarea').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }


    }
    else if ($('#idst').val() == 2) {

        if ($('#bankid').val() == '' || $('#bankid').val() == null) {
            $(".errorss").text('Select Bank Name');
            return false;
        }
        if ($('#txtsupramount').val() == '' || $('#txtsupramount').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtsuprdeposit').val() == '' || $('#txtsuprdeposit').val() == null) {
            $(".errorss").text('Enter Deposite Slip No.');
            return false;
        }

        if ($('#txtsuprarea').val() == '' || $('#txtsuprarea').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }

    }

    else if ($('#idst').val() == 3) {

        if ($('#ddlsuprtransfertype').val() == '' || $('#ddlsuprtransfertype').val() == null) {
            $(".errorss").text('Select Transfer Type');
            return false;
        }
        if ($('#bankid').val() == '' || $('#bankid').val() == null) {
            $(".errorss").text('Select Bank Name');
            return false;
        }
        if ($('#txtsupraccno').val() == '' || $('#txtsupraccno').val() == null) {
            $(".errorss").text('Account No Required');
            return false;
        }
        if ($('#txtsuprutr').val() == '' || $('#txtsuprutr').val() == null) {
            $(".errorss").text('UTR No Required');
            return false;
        }
        if ($('#txtsupramount').val() == '' || $('#txtsupramount').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }

    }
    else if ($('#idst').val() == 4) {

        if ($('#ddlsuprwalletname').val() == '' || $('#ddlsuprwalletname').val() == null) {
            $(".errorss").text('Collection Person Name');
            return false;
        }
        if ($('#txtsuprwalletno').val() == '' || $('#txtsuprwalletno').val() == null) {
            $(".errorss").text('Wallet No. Required');
            return false;
        }
        if ($('#txtsuprtransno').val() == '' || $('#txtsuprtransno').val() == null) {
            $(".errorss").text('Transtion No.');
            return false;
        }
        if ($('#txtsupramount').val() == '' || $('#txtsupramount').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtsuprarea').val() == '' || $('#txtsuprarea').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }
    else if ($('#idst').val() == 5) {

        if ($('#txtsupramount').val() == '' || $('#txtsupramount').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtsuprarea').val() == '' || $('#txtsuprarea').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }

    }
    else if ($('#idst').val() == 6) {

        if ($('#txtsuprsettl').val() == '' || $('#txtsuprsettl').val() == null) {
            $(".errorss").text('Settlement Type');
            return false;
        }
        if ($('#txtsupramount').val() == '' || $('#txtsupramount').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtsuprarea').val() == '' || $('#txtsuprarea').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }
    else if ($('#idst').val() == 7) {

        if ($('#txtsuprcrdetail').val() == '' || $('#txtsuprcrdetail').val() == null) {
            $(".errorss").text('Credit Detail Required');
            return false;
        }
        if ($('#txtsupramount').val() == '' || $('#txtsupramount').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtsuprarea').val() == '' || $('#txtsuprarea').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }
    else if ($('#idst').val() == 8) {

        if ($('#txtsuprsubreason').val() == '' || $('#txtsuprsubreason').val() == null) {
            $(".errorss").text('Enter Debit Reason');
            return false;
        }
        if ($('#txtsupramount').val() == '' || $('#txtsupramount').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtsuprarea').val() == '' || $('#txtsuprarea').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }
    else if ($('#idst').val() == 9) {

        if ($('#txtsuprsubreason').val() == '' || $('#txtsuprsubreason').val() == null) {
            $(".errorss").text('Enter Debit Reason');
            return false;
        }
        if ($('#txtsupramount').val() == '' || $('#txtsupramount').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtsuprarea').val() == '' || $('#txtsuprarea').val() == null) {
            $(".errorss").text('Comment Required');
            return false;
        }
    }



    $('label').next('.est').remove()

    var name = $("#SuperstokistID option:selected");
    var idst = $("#idst option:selected").text();
    var collper = $('#txtsuprcollperson').val();
    var amount = $('#txtsupramount').val();

    var comment = $('#txtsuprarea').val();
    var bankname = $("#bankid option:selected").text();
    var depositeslip = $('#txtsuprdeposit').val();
    var tranftype = $("#ddlsuprtransfertype option:selected");
    var accountno = $('#txtsupraccno').val();
    var UTRnos = $('#txtsuprutr').val();
    var walletname = $("#ddlsuprwalletname option:selected");
    var walletno = $('#txtsuprwalletno').val();
    var transectionno = $('#txtsuprtransno').val();
    var setteltype = $('#txtsuprsettl').val();
    var crdetails = $('#txtsuprcrdetail').val();
    var sbject = $('#txtsuprsubreason').val();

    var accnosele = $("#bankid").val();













    //   $('#MasterDistributorName').after('<label class="est"><span>:<span>&nbsp; '+name.text()+'</label>')
    // $('#PaymentMode').after('<label class="est"><span>:<span>&nbsp; '+idst+'</label>')
    // $('#PaymentAmount').after('<label class="est"><span>:<span>&nbsp; '+amount+'</label>')



    var thtm = '<div id="hdidsss">';
    thtm += '<input id="hdSuperstokistID" name="hdSuperstokistID" type="hidden" name="classid" value="' + $('#SuperstokistID').val() + '" />';
    thtm += '<input id="hdPaymentMode" name="hdPaymentMode" type="hidden"  value="' + idst + '" />';
    //  thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' +amount + '" />';
    $('#m-collection').parent().hide();
    $('#m-ammount').parent().hide();
    $('#m-comment').parent().hide();
    $('#m-Bank').parent().hide();
    $('#m-ammount').parent().hide();
    $('#m-DepositeSlipNo').parent().hide();
    $('#m-comment').parent().hide();
    $('#m-Bank').parent().hide();
    $('#m-ammount').parent().hide();
    $('#m-DepositeSlipNo').parent().hide();
    $('#m-comment').parent().hide();
    $('#m-utr-no').parent().hide();
    $('#m-subject').parent().hide();
    $('#m-comment').parent().hide();
    $('#m-subject').parent().hide();
    $('#m-account-no').parent().hide();
    $('#m-TransferType').parent().hide();
    $('#m-wallet').parent().hide();
    $('#m-wallet-no').parent().hide();
    $('#m-transation-no').parent().hide();
    $('#m-CreditDetail').parent().hide();
    $('#m-settelment').parent().hide();
    $('#m-wallet-no').parent().hide();
    $('#m-name').text(name.text())
    $('#m-Mode').text(idst)

    if ($("#idst option:selected").index() === 0) {
        $('#m-collection').parent().show();
        $('#m-ammount').parent().show();
        $('#m-comment').parent().show();

        $('#m-collection').text(collper)
        $('#m-ammount').text(amount)
        $('#m-comment').text(comment)
        thtm += '<input id="hdMDcollection" name="hdMDcollection" type="hidden"  value="' + collper + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#idst option:selected").index() === 1) {
        $('#m-Bank').parent().show();
        $('#m-ammount').parent().show();
        $('#m-DepositeSlipNo').parent().show();
        $('#m-comment').parent().show();


        $('#m-Bank').text(bankname)
        $('#m-ammount').text(amount)
        $('#m-DepositeSlipNo').text(depositeslip);
        $('#m-comment').text(comment)

        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accnosele + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDDepositeSlipNo" name="hdMDDepositeSlipNo" type="hidden"  value="' + depositeslip + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#idst option:selected").index() === 2) {
        $('#m-TransferType').parent().show();
        $('#m-Bank').parent().show();
        $('#m-ammount').parent().show();
        $('#m-account-no').parent().show();
        $('#m-utr-no').parent().show();

        $('#m-TransferType').text(tranftype.text());
        $('#m-Bank').text(bankname)
        $('#m-account-no').text(accountno);
        $('#m-utr-no').text(UTRnos)
        $('#m-ammount').text(amount)
        thtm += '<input id="hdMDTransferType" name="hdMDTransferType" type="hidden"  value="' + tranftype.text() + '" />';
        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accountno + '" />';
        thtm += '<input id="hdMDutrno" name="hdMDutrno" type="hidden"  value="' + UTRnos + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#idst option:selected").index() === 3) {
        $('#m-wallet').parent().show();
        $('#m-wallet-no').parent().show();
        $('#m-transation-no').parent().show();
        $('#m-ammount').parent().show();
        $('#m-comment').parent().show();

        $('#m-wallet').text(walletname.text())
        $('#m-wallet-no').text(walletno)
        $('#m-transation-no').text(transectionno)
        $('#m-ammount').text(amount)
        $('#m-comment').text(comment)
        thtm += '<input id="hdMDwallet" name="hdMDwallet" type="hidden"  value="' + walletname.text() + '" />';
        thtm += '<input id="hdMDwalletno" name="hdMDwalletno" type="hidden"  value="' + walletno + '" />';
        thtm += '<input id="hdMDtransationno" name="hdMDtransationno" type="hidden"  value="' + transectionno + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#idst option:selected").index() === 4) {
        $('#m-ammount').parent().show();
        $('#m-comment').parent().show();

        $('#m-ammount').text(amount)
        $('#m-comment').text(comment)


        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#idst option:selected").index() === 5) {
        $('#m-settelment').parent().show();
        $('#m-ammount').parent().show();
        $('#m-comment').parent().show();

        $('#m-settelment').text(setteltype);
        $('#m-ammount').text(amount)
        $('#m-comment').text(comment)
        thtm += '<input id="hdMDsettelment" name="hdMDsettelment" type="hidden"  value="' + setteltype + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#idst option:selected").index() === 6) {
        $('#m-CreditDetail').parent().show();
        $('#m-ammount').parent().show();
        $('#m-comment').parent().show();



        $('#m-CreditDetail').text(crdetails)
        $('#m-ammount').text(amount)
        $('#m-comment').text(comment)

        thtm += '<input id="hdMDCreditDetail" name="hdMDCreditDetail" type="hidden"  value="' + crdetails + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';

    }
    if ($("#idst option:selected").index() === 7 || $("#idst option:selected").index() === 8) {
        $('#m-subject').parent().show();
        $('#m-ammount').parent().show();
        $('#m-comment').parent().show();


        $('#m-subject').text(sbject)
        $('#m-ammount').text(amount)
        $('#m-comment').text(comment)
        thtm += '<input id="hdMDsubject" name="hdMDsubject" type="hidden"  value="' + sbject + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    thtm += '</div>';
    $('#frmdata').hide();

    $('.txtcodecodec').val('');
    $('#showinformation').show();
    $('#CallMASTER').show();
    //   thtm += '</div>';
    // $('#frmpassword').html(thtm)
    //var thtm = '<div id="hid1"><input type="hidden" name="classid" value="0" />';
    //thtm += '<div id="hid2"><input type="hidden" name="classid" value="0" />';
    //thtm = '<div id="hid3"><input type="hidden" name="classid" value="0" />';
    //thtm = '<div id="hid4"><input type="hidden" name="classid" value="0" />';
    //thtm = '<div id="hid5"><input type="hidden" name="classid" value="0" />';
    $('#hdninputvalues').html(thtm);


    //      $("#showinform").append('<form action="sharer.php" method="POST">');
    //$("#showinform").append('<div class="appm">Save this</div>');
    //$("#showinform").append('<input type="text" placeholder="Name" name="routename" id="rname"/>');
    //$("#showinform").append('<input type="text" placeholder="description" id="rdescription" name="routedescription" class="address"/>');
    //$("#share form").append('<input type="text" placeholder="tags" id="tags" name="routetags"/>');
    //$("#showinform").append('<br><input type="submit" id="savebutton" value="Save" />');

})
///Super TockList END

////sdsfssddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd

function changemaster(value) {

    var mas = $(value).val();
    if (mas == '') {
        $('.old-cr').hide();

    } else {
        $('.old-cr').show();
    }
}
function changemode(value) {
    var res = $(value).val();
    if (res == 1) {
        $('.collection-by').show();
        $('.transfer-type').hide();
        $('.bank-select').hide();
        $('.account-no').hide();
        $('.utr-no').hide();
        $('.wallet-select').hide();
        $('.wallet-no').hide();
        $('.transtion-no').hide();
        $('.settle-type').hide();
        $('.credit-detail').hide();
        $('.subject-reason').hide();
        $('.deposite-slip-no').hide();
        $('.fund-comment').show();
    }
    else if (res == 2) {
        $('.collection-by').hide();
        $('.transfer-type').hide();
        $('.bank-select').show();
        $('.account-no').hide();
        $('.utr-no').hide();
        $('.wallet-select').hide();
        $('.wallet-no').hide();
        $('.transtion-no').hide();
        $('.settle-type').hide();
        $('.credit-detail').hide();
        $('.subject-reason').hide();
        $('.deposite-slip-no').show();
        $('.fund-comment').show();
    }
    else if (res == 3) {
        $('.collection-by').hide();
        $('.transfer-type').show();
        $('.bank-select').show();
        $('.account-no').show();
        $('.utr-no').show();
        $('.wallet-select').hide();
        $('.wallet-no').hide();
        $('.transtion-no').hide();
        $('.settle-type').hide();
        $('.credit-detail').hide();
        $('.subject-reason').hide();
        $('.deposite-slip-no').hide();
        $('.fund-comment').hide();


    }
    else if (res == 4) {
        $('.collection-by').hide();
        $('.transfer-type').hide();
        $('.bank-select').hide();
        $('.account-no').hide();
        $('.utr-no').hide();
        $('.wallet-select').show();
        $('.wallet-no').show();
        $('.transtion-no').show();
        $('.settle-type').hide();
        $('.credit-detail').hide();
        $('.subject-reason').hide();
        $('.deposite-slip-no').hide();
        $('.fund-comment').show();
    }
    else if (res == 5) {
        $('.collection-by').hide();
        $('.transfer-type').hide();
        $('.bank-select').hide();
        $('.account-no').hide();
        $('.utr-no').hide();
        $('.wallet-select').hide();
        $('.wallet-no').hide();
        $('.transtion-no').hide();
        $('.settle-type').hide();
        $('.credit-detail').hide();
        $('.subject-reason').hide();
        $('.deposite-slip-no').hide();
        $('.fund-comment').show();
    }
    else if (res == 6) {
        $('.collection-by').hide();
        $('.transfer-type').hide();
        $('.bank-select').hide();
        $('.account-no').hide();
        $('.utr-no').hide();
        $('.wallet-select').hide();
        $('.wallet-no').hide();
        $('.transtion-no').hide();
        $('.settle-type').show();
        $('.credit-detail').hide();
        $('.subject-reason').hide();
        $('.deposite-slip-no').hide();
        $('.fund-comment').show();
    }
    else if (res == 7) {
        $('.collection-by').hide();
        $('.transfer-type').hide();
        $('.bank-select').hide();
        $('.account-no').hide();
        $('.utr-no').hide();
        $('.wallet-select').hide();
        $('.wallet-no').hide();
        $('.transtion-no').hide();
        $('.settle-type').hide();
        $('.credit-detail').show();
        $('.subject-reason').hide();
        $('.deposite-slip-no').hide();
        $('.fund-comment').show();
    }
    else if (res == 8) {
        $('.collection-by').hide();
        $('.transfer-type').hide();
        $('.bank-select').hide();
        $('.account-no').hide();
        $('.utr-no').hide();
        $('.wallet-select').hide();
        $('.wallet-no').hide();
        $('.transtion-no').hide();
        $('.settle-type').hide();
        $('.credit-detail').hide();
        $('.subject-reason').show();
        $('.deposite-slip-no').hide();
        $('.fund-comment').show();
    }
    else if (res == 9) {
        $('.collection-by').hide();
        $('.transfer-type').hide();
        $('.bank-select').hide();
        $('.account-no').hide();
        $('.utr-no').hide();
        $('.wallet-select').hide();
        $('.wallet-no').hide();
        $('.transtion-no').hide();
        $('.settle-type').hide();
        $('.credit-detail').hide();
        $('.subject-reason').show();
        $('.deposite-slip-no').hide();
        $('.fund-comment').show();
    }
}

function changemasterd(value) {
    var mas = $(value).val();
    if (mas == '') {
        $('.old-crd').hide();

    } else {
        $('.old-crd').show();
    }
}
function changemoded(value) {
    var res = $(value).val();
    if (res == 1) {
        $('.dcollection-by').show();
        $('.dtransfer-type').hide();
        $('.dbank-select').hide();
        $('.daccount-no').hide();
        $('.dutr-no').hide();
        $('.dwallet-select').hide();
        $('.dwallet-no').hide();
        $('.dtranstion-no').hide();
        $('.dsettle-type').hide();
        $('.dcredit-detail').hide();
        $('.dsubject-reason').hide();
        $('.ddeposite-slip-no').hide();
        $('.dfund-comment').show();
    }
    else if (res == 2) {
        $('.dcollection-by').hide();
        $('.dtransfer-type').hide();
        $('.dbank-select').show();
        $('.daccount-no').hide();
        $('.dutr-no').hide();
        $('.dwallet-select').hide();
        $('.dwallet-no').hide();
        $('.dtranstion-no').hide();
        $('.dsettle-type').hide();
        $('.dcredit-detail').hide();
        $('.dsubject-reason').hide();
        $('.ddeposite-slip-no').show();
        $('.dfund-comment').show();
    }
    else if (res == 3) {
        $('.dcollection-by').hide();
        $('.dtransfer-type').show();
        $('.dbank-select').show();
        $('.daccount-no').show();
        $('.dutr-no').show();
        $('.dwallet-select').hide();
        $('.dwallet-no').hide();
        $('.dtranstion-no').hide();
        $('.dsettle-type').hide();
        $('.dcredit-detail').hide();
        $('.dsubject-reason').hide();
        $('.ddeposite-slip-no').hide();
        $('.dfund-comment').hide();


    }
    else if (res == 4) {
        $('.dcollection-by').hide();
        $('.dtransfer-type').hide();
        $('.dbank-select').hide();
        $('.daccount-no').hide();
        $('.dutr-no').hide();
        $('.dwallet-select').show();
        $('.dwallet-no').show();
        $('.dtranstion-no').show();
        $('.dsettle-type').hide();
        $('.dcredit-detail').hide();
        $('.dsubject-reason').hide();
        $('.ddeposite-slip-no').hide();
        $('.dfund-comment').show();
    }
    else if (res == 5) {
        $('.dcollection-by').hide();
        $('.dtransfer-type').hide();
        $('.dbank-select').hide();
        $('.daccount-no').hide();
        $('.dutr-no').hide();
        $('.dwallet-select').hide();
        $('.dwallet-no').hide();
        $('.dtranstion-no').hide();
        $('.dsettle-type').hide();
        $('.dcredit-detail').hide();
        $('.dsubject-reason').hide();
        $('.ddeposite-slip-no').hide();
        $('.dfund-comment').show();
    }
    else if (res == 6) {
        $('.dcollection-by').hide();
        $('.dtransfer-type').hide();
        $('.dbank-select').hide();
        $('.daccount-no').hide();
        $('.dutr-no').hide();
        $('.dwallet-select').hide();
        $('.dwallet-no').hide();
        $('.dtranstion-no').hide();
        $('.dsettle-type').show();
        $('.dcredit-detail').hide();
        $('.dsubject-reason').hide();
        $('.ddeposite-slip-no').hide();
        $('.dfund-comment').show();
    }
    else if (res == 7) {
        $('.dcollection-by').hide();
        $('.dtransfer-type').hide();
        $('.dbank-select').hide();
        $('.daccount-no').hide();
        $('.dutr-no').hide();
        $('.dwallet-select').hide();
        $('.dwallet-no').hide();
        $('.dtranstion-no').hide();
        $('.dsettle-type').hide();
        $('.dcredit-detail').show();
        $('.dsubject-reason').hide();
        $('.ddeposite-slip-no').hide();
        $('.dfund-comment').show();
    }
    else if (res == 8) {
        $('.dcollection-by').hide();
        $('.dtransfer-type').hide();
        $('.dbank-select').hide();
        $('.daccount-no').hide();
        $('.dutr-no').hide();
        $('.dwallet-select').hide();
        $('.dwallet-no').hide();
        $('.dtranstion-no').hide();
        $('.dsettle-type').hide();
        $('.dcredit-detail').hide();
        $('.dsubject-reason').show();
        $('.ddeposite-slip-no').hide();
        $('.dfund-comment').show();
    }
    else if (res == 9) {
        $('.dcollection-by').hide();
        $('.dtransfer-type').hide();
        $('.dbank-select').hide();
        $('.daccount-no').hide();
        $('.dutr-no').hide();
        $('.dwallet-select').hide();
        $('.dwallet-no').hide();
        $('.dtranstion-no').hide();
        $('.dsettle-type').hide();
        $('.dcredit-detail').hide();
        $('.dsubject-reason').show();
        $('.ddeposite-slip-no').hide();
        $('.dfund-comment').show();
    }
}

function changemasterr(value) {
    var mas = $(value).val();
    if (mas == 1) {
        $('.old-crr').hide();

    } else {
        $('.old-crr').show();
    }
}
function changemoder(value) {
    var res = $(value).val();
    if (res == 1) {
        $('.rcollection-by').show();
        $('.rtransfer-type').hide();
        $('.rbank-select').hide();
        $('.raccount-no').hide();
        $('.rutr-no').hide();
        $('.rwallet-select').hide();
        $('.rwallet-no').hide();
        $('.rtranstion-no').hide();
        $('.rsettle-type').hide();
        $('.rcredit-detail').hide();
        $('.rsubject-reason').hide();
        $('.rdeposite-slip-no').hide();
        $('.rfund-comment').show();
    }
    else if (res == 2) {
        $('.rcollection-by').hide();
        $('.rtransfer-type').hide();
        $('.rbank-select').show();
        $('.raccount-no').hide();
        $('.rutr-no').hide();
        $('.rwallet-select').hide();
        $('.rwallet-no').hide();
        $('.rtranstion-no').hide();
        $('.rsettle-type').hide();
        $('.rcredit-detail').hide();
        $('.rsubject-reason').hide();
        $('.rdeposite-slip-no').show();
        $('.rfund-comment').show();
    }
    else if (res == 3) {
        $('.rcollection-by').hide();
        $('.rtransfer-type').show();
        $('.rbank-select').show();
        $('.raccount-no').show();
        $('.rutr-no').show();
        $('.rwallet-select').hide();
        $('.rwallet-no').hide();
        $('.rtranstion-no').hide();
        $('.rsettle-type').hide();
        $('.rcredit-detail').hide();
        $('.rsubject-reason').hide();
        $('.rdeposite-slip-no').hide();
        $('.rfund-comment').hide();


    }
    else if (res == 4) {
        $('.rcollection-by').hide();
        $('.rtransfer-type').hide();
        $('.rbank-select').hide();
        $('.raccount-no').hide();
        $('.rutr-no').hide();
        $('.rwallet-select').show();
        $('.rwallet-no').show();
        $('.rtranstion-no').show();
        $('.rsettle-type').hide();
        $('.rcredit-detail').hide();
        $('.rsubject-reason').hide();
        $('.rdeposite-slip-no').hide();
        $('.rfund-comment').show();
    }
    else if (res == 5) {
        $('.rcollection-by').hide();
        $('.rtransfer-type').hide();
        $('.rbank-select').hide();
        $('.raccount-no').hide();
        $('.rutr-no').hide();
        $('.rwallet-select').hide();
        $('.rwallet-no').hide();
        $('.rtranstion-no').hide();
        $('.rsettle-type').hide();
        $('.rcredit-detail').hide();
        $('.rsubject-reason').hide();
        $('.rdeposite-slip-no').hide();
        $('.rfund-comment').show();
    }
    else if (res == 6) {
        $('.rcollection-by').hide();
        $('.rtransfer-type').hide();
        $('.rbank-select').hide();
        $('.raccount-no').hide();
        $('.rutr-no').hide();
        $('.rwallet-select').hide();
        $('.rwallet-no').hide();
        $('.rtranstion-no').hide();
        $('.rsettle-type').show();
        $('.rcredit-detail').hide();
        $('.rsubject-reason').hide();
        $('.rdeposite-slip-no').hide();
        $('.rfund-comment').show();
    }
    else if (res == 7) {
        $('.rcollection-by').hide();
        $('.rtransfer-type').hide();
        $('.rbank-select').hide();
        $('.raccount-no').hide();
        $('.rutr-no').hide();
        $('.rwallet-select').hide();
        $('.rwallet-no').hide();
        $('.rtranstion-no').hide();
        $('.rsettle-type').hide();
        $('.rcredit-detail').show();
        $('.rsubject-reason').hide();
        $('.rdeposite-slip-no').hide();
        $('.rfund-comment').show();
    }
    else if (res == 8) {
        $('.rcollection-by').hide();
        $('.rtransfer-type').hide();
        $('.rbank-select').hide();
        $('.raccount-no').hide();
        $('.rutr-no').hide();
        $('.rwallet-select').hide();
        $('.rwallet-no').hide();
        $('.rtranstion-no').hide();
        $('.rsettle-type').hide();
        $('.rcredit-detail').hide();
        $('.rsubject-reason').show();
        $('.rdeposite-slip-no').hide();
        $('.rfund-comment').show();
    }
    else if (res == 9) {
        $('.rcollection-by').hide();
        $('.rtransfer-type').hide();
        $('.rbank-select').hide();
        $('.raccount-no').hide();
        $('.rutr-no').hide();
        $('.rwallet-select').hide();
        $('.rwallet-no').hide();
        $('.rtranstion-no').hide();
        $('.rsettle-type').hide();
        $('.rcredit-detail').hide();
        $('.rsubject-reason').show();
        $('.rdeposite-slip-no').hide();
        $('.rfund-comment').show();
    }
}

function changemastera(value) {
    var mas = $(value).val();
    if (mas == 1) {
        $('.old-cra').hide();

    } else {
        $('.old-cra').show();
    }
}
function changemodea(value) {
    var res = $(value).val();
    if (res == 1) {
        $('.acollection-by').show();
        $('.atransfer-type').hide();
        $('.abank-select').hide();
        $('.aaccount-no').hide();
        $('.autr-no').hide();
        $('.awallet-select').hide();
        $('.awallet-no').hide();
        $('.atranstion-no').hide();
        $('.asettle-type').hide();
        $('.acredit-detail').hide();
        $('.asubject-reason').hide();
        $('.adeposite-slip-no').hide();
        $('.afund-comment').show();
    }
    else if (res == 2) {
        $('.acollection-by').hide();
        $('.atransfer-type').hide();
        $('.abank-select').show();
        $('.aaccount-no').hide();
        $('.autr-no').hide();
        $('.awallet-select').hide();
        $('.awallet-no').hide();
        $('.atranstion-no').hide();
        $('.asettle-type').hide();
        $('.acredit-detail').hide();
        $('.asubject-reason').hide();
        $('.adeposite-slip-no').show();
        $('.afund-comment').show();
    }
    else if (res == 3) {
        $('.acollection-by').hide();
        $('.atransfer-type').show();
        $('.abank-select').show();
        $('.aaccount-no').show();
        $('.autr-no').show();
        $('.awallet-select').hide();
        $('.awallet-no').hide();
        $('.atranstion-no').hide();
        $('.asettle-type').hide();
        $('.acredit-detail').hide();
        $('.asubject-reason').hide();
        $('.adeposite-slip-no').hide();
        $('.afund-comment').hide();


    }
    else if (res == 4) {
        $('.acollection-by').hide();
        $('.atransfer-type').hide();
        $('.abank-select').hide();
        $('.aaccount-no').hide();
        $('.autr-no').hide();
        $('.awallet-select').show();
        $('.awallet-no').show();
        $('.atranstion-no').show();
        $('.asettle-type').hide();
        $('.acredit-detail').hide();
        $('.asubject-reason').hide();
        $('.adeposite-slip-no').hide();
        $('.afund-comment').show();
    }
    else if (res == 5) {
        $('.acollection-by').hide();
        $('.atransfer-type').hide();
        $('.abank-select').hide();
        $('.aaccount-no').hide();
        $('.autr-no').hide();
        $('.awallet-select').hide();
        $('.awallet-no').hide();
        $('.atranstion-no').hide();
        $('.asettle-type').hide();
        $('.acredit-detail').hide();
        $('.asubject-reason').hide();
        $('.adeposite-slip-no').hide();
        $('.afund-comment').show();
    }
    else if (res == 6) {
        $('.acollection-by').hide();
        $('.atransfer-type').hide();
        $('.abank-select').hide();
        $('.aaccount-no').hide();
        $('.autr-no').hide();
        $('.awallet-select').hide();
        $('.awallet-no').hide();
        $('.atranstion-no').hide();
        $('.asettle-type').show();
        $('.acredit-detail').hide();
        $('.asubject-reason').hide();
        $('.adeposite-slip-no').hide();
        $('.afund-comment').show();
    }
    else if (res == 7) {
        $('.acollection-by').hide();
        $('.atransfer-type').hide();
        $('.abank-select').hide();
        $('.aaccount-no').hide();
        $('.autr-no').hide();
        $('.awallet-select').hide();
        $('.awallet-no').hide();
        $('.atranstion-no').hide();
        $('.asettle-type').hide();
        $('.acredit-detail').show();
        $('.asubject-reason').hide();
        $('.adeposite-slip-no').hide();
        $('.afund-comment').show();
    }
    else if (res == 8) {
        $('.acollection-by').hide();
        $('.atransfer-type').hide();
        $('.abank-select').hide();
        $('.aaccount-no').hide();
        $('.autr-no').hide();
        $('.awallet-select').hide();
        $('.awallet-no').hide();
        $('.atranstion-no').hide();
        $('.asettle-type').hide();
        $('.acredit-detail').hide();
        $('.asubject-reason').show();
        $('.adeposite-slip-no').hide();
        $('.afund-comment').show();
    }
    else if (res == 9) {
        $('.acollection-by').hide();
        $('.atransfer-type').hide();
        $('.abank-select').hide();
        $('.aaccount-no').hide();
        $('.autr-no').hide();
        $('.awallet-select').hide();
        $('.awallet-no').hide();
        $('.atranstion-no').hide();
        $('.asettle-type').hide();
        $('.acredit-detail').hide();
        $('.asubject-reason').show();
        $('.adeposite-slip-no').hide();
        $('.afund-comment').show();
    }
}

function changemasterw(value) {
    var mas = $(value).val();
    if (mas == 1) {
        $('.old-crw').hide();

    } else {
        $('.old-crw').show();
    }
}
function changemodew(value) {
    var res = $(value).val();
    if (res == 1) {
        $('.wcollection-by').show();
        $('.wtransfer-type').hide();
        $('.wbank-select').hide();
        $('.waccount-no').hide();
        $('.wutr-no').hide();
        $('.wwallet-select').hide();
        $('.wwallet-no').hide();
        $('.wtranstion-no').hide();
        $('.wsettle-type').hide();
        $('.wcredit-detail').hide();
        $('.wsubject-reason').hide();
        $('.wdeposite-slip-no').hide();
        $('.wfund-comment').show();
    }
    else if (res == 2) {
        $('.wcollection-by').hide();
        $('.wtransfer-type').hide();
        $('.wbank-select').show();
        $('.waccount-no').hide();
        $('.wutr-no').hide();
        $('.wwallet-select').hide();
        $('.wwallet-no').hide();
        $('.wtranstion-no').hide();
        $('.wsettle-type').hide();
        $('.wcredit-detail').hide();
        $('.wsubject-reason').hide();
        $('.wdeposite-slip-no').show();
        $('.wfund-comment').show();
    }
    else if (res == 3) {
        $('.wcollection-by').hide();
        $('.wtransfer-type').show();
        $('.wbank-select').show();
        $('.waccount-no').show();
        $('.wutr-no').show();
        $('.wwallet-select').hide();
        $('.wwallet-no').hide();
        $('.wtranstion-no').hide();
        $('.wsettle-type').hide();
        $('.wcredit-detail').hide();
        $('.wsubject-reason').hide();
        $('.wdeposite-slip-no').hide();
        $('.wfund-comment').hide();


    }
    else if (res == 4) {
        $('.wcollection-by').hide();
        $('.wtransfer-type').hide();
        $('.wbank-select').hide();
        $('.waccount-no').hide();
        $('.wutr-no').hide();
        $('.wwallet-select').show();
        $('.wwallet-no').show();
        $('.wtranstion-no').show();
        $('.wsettle-type').hide();
        $('.wcredit-detail').hide();
        $('.wsubject-reason').hide();
        $('.wdeposite-slip-no').hide();
        $('.wfund-comment').show();
    }
    else if (res == 5) {
        $('.wcollection-by').hide();
        $('.wtransfer-type').hide();
        $('.wbank-select').hide();
        $('.waccount-no').hide();
        $('.wutr-no').hide();
        $('.wwallet-select').hide();
        $('.wwallet-no').hide();
        $('.wtranstion-no').hide();
        $('.wsettle-type').hide();
        $('.wcredit-detail').hide();
        $('.wsubject-reason').hide();
        $('.wdeposite-slip-no').hide();
        $('.wfund-comment').show();
    }
    else if (res == 6) {
        $('.wcollection-by').hide();
        $('.wtransfer-type').hide();
        $('.wbank-select').hide();
        $('.waccount-no').hide();
        $('.wutr-no').hide();
        $('.wwallet-select').hide();
        $('.wwallet-no').hide();
        $('.wtranstion-no').hide();
        $('.wsettle-type').show();
        $('.wcredit-detail').hide();
        $('.wsubject-reason').hide();
        $('.wdeposite-slip-no').hide();
        $('.wfund-comment').show();
    }
    else if (res == 7) {
        $('.wcollection-by').hide();
        $('.wtransfer-type').hide();
        $('.wbank-select').hide();
        $('.waccount-no').hide();
        $('.wutr-no').hide();
        $('.wwallet-select').hide();
        $('.wwallet-no').hide();
        $('.wtranstion-no').hide();
        $('.wsettle-type').hide();
        $('.wcredit-detail').show();
        $('.wsubject-reason').hide();
        $('.wdeposite-slip-no').hide();
        $('.wfund-comment').show();
    }
    else if (res == 8) {
        $('.wcollection-by').hide();
        $('.wtransfer-type').hide();
        $('.wbank-select').hide();
        $('.waccount-no').hide();
        $('.wutr-no').hide();
        $('.wwallet-select').hide();
        $('.wwallet-no').hide();
        $('.wtranstion-no').hide();
        $('.wsettle-type').hide();
        $('.wcredit-detail').hide();
        $('.wsubject-reason').show();
        $('.wdeposite-slip-no').hide();
        $('.wfund-comment').show();
    }
    else if (res == 9) {
        $('.wcollection-by').hide();
        $('.wtransfer-type').hide();
        $('.wbank-select').hide();
        $('.waccount-no').hide();
        $('.wutr-no').hide();
        $('.wwallet-select').hide();
        $('.wwallet-no').hide();
        $('.wtranstion-no').hide();
        $('.wsettle-type').hide();
        $('.wcredit-detail').hide();
        $('.wsubject-reason').show();
        $('.wdeposite-slip-no').hide();
        $('.wfund-comment').show();
    }
}

$(document).ready(function () {
    $(".errorss").text('');

    $('ul#tabs li').click(function () {
        $(".errorss").text('');
        var tab_id = $(this).attr('data-tab');

        $('ul#tabs li').removeClass('current');

        $('.tab-content').removeClass('current');

        $(this).addClass('current');
        $("#" + tab_id).addClass('current');
    })

})
