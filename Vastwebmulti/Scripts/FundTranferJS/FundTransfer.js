
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
$('#btnWLbackshow').click(function () {
    $("input:text").val("");
    $('#formWLUSER').show();
    $('#hdWLninputvalues').empty();

    $('#WLUSERshowinformation').hide();

})
$('#btnAPIbackshow').click(function () {
    $("input:text").val("");
    $('#formAPIUSER').show();
    $('#hdAPIninputvalues').empty();

    $('#APIUSERshowinformation').hide();

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

$('#APIRIbankid').change(function () {
    var acno = $('#APIRIbankid').val();
    $('#txtAPIACCOUNTNO').val(acno)

})
$('#ddlAPIwalletname').change(function () {
    var acno = $('#ddlAPIwalletname').val();
    $('#txtAPIWALLETNO').val(acno)

})

$('#WLRIbankid').change(function () {
    var acno = $('#WLRIbankid').val();
    $('#txtWLACCOUNTNO').val(acno)

})
$('#ddlWLwalletname').change(function () {
    var acno = $('#ddlWLwalletname').val();
    $('#txtWLWALLETNO').val(acno)

})



///////////////////////////////////////////////////////////////////////////


///White LAbel Start





///API Start

$('#btnWLUSERInform').click(function () {

    var callurl = $('#btnWLUSERInform').attr('data-buttoncall')
    /*  alert(callurl)*/

    $('#hdWLninputvalues').empty();
    $('#tblshowinformadmintoWebapi').empty();

    if ($('#WLUSER').val() == '' || $('#WLUSER').val() == null) {
        $("#formWLUSER .errorss").text('Select Whitelabel User');
        return false;
    }
    if ($('#ddlWLPaymentModel').val() == '' || $('#ddlWLPaymentModel').val() == null) {
        $("#formWLUSER .errorss").text('Pay mode REQUIRED');
        return false;
    }
    if ($('#ddlWLPaymentModel').val() == 1) {
        if ($('#txtWLCollectionBy').val() == '' || $('#txtWLCollectionBy').val() == null) {
            $('#txtWLCollectionBy').val('Self')
        }
        if ($('#txtWLAMOUNT').val() == '' || $('#txtWLAMOUNT').val() == null) {
            $("#formWLUSER .errorss").text('Amount Required');
            return false;
        }
        if ($('#txtWLCOMMENTS').val() == '' || $('#txtWLCOMMENTS').val() == null) {
            $('#txtWLCOMMENTS').val('No Comments')
        }


    }
    else if ($('#ddlWLPaymentModel').val() == 2) {

        if ($('#WLRIbankid').val() == '' || $('#WLRIbankid').val() == null) {
            $("#formWLUSER .errorss").text('Select Bank Name');
            return false;
        }
        if ($('#txtWLAMOUNT').val() == '' || $('#txtWLAMOUNT').val() == null) {
            $("#formWLUSER .errorss").text('Amount Required');
            return false;
        }
        if ($('#txtWLDEPOSITESLIPNO').val() == '' || $('#txtWLDEPOSITESLIPNO').val() == null) {
            $("#formWLUSER .errorss").text('Enter Deposite Slip No.');
            return false;
        }

        if ($('#txtWLCOMMENTS').val() == '' || $('#txtWLCOMMENTS').val() == null) {
            $('#txtWLCOMMENTS').val('No Comments')
        }

    }

    else if ($('#ddlWLPaymentModel').val() == 3) {

        if ($('#WLTransferType').val() == '' || $('#WLTransferType').val() == null) {
            $("#formWLUSER .errorss").text('Select Transfer Type');
            return false;
        }
        if ($('#WLRIbankid').val() == '' || $('#WLRIbankid').val() == null) {
            $("#formWLUSER .errorss").text('Select Bank Name');
            return false;
        }
        if ($('#txtWLACCOUNTNO').val() == '' || $('#txtWLACCOUNTNO').val() == null) {
            $("#formWLUSER .errorss").text('Account No Required');
            return false;
        }
        if ($('#txtWLUTRNO').val() == '' || $('#txtWLUTRNO').val() == null) {
            $("#formWLUSER .errorss").text('UTR No Required');
            return false;
        }
        if ($('#txtWLAMOUNT').val() == '' || $('#txtWLAMOUNT').val() == null) {
            $("#formWLUSER .errorss").text('Amount Required');
            return false;
        }

    }
    else if ($('#ddlWLPaymentModel').val() == 4) {

        if ($('#ddlWLwalletname').val() == '' || $('#ddlWLwalletname').val() == null) {
            $("#formWLUSER .errorss").text('Select Wallet Name');
            return false;
        }
        if ($('#txtWLWALLETNO').val() == '' || $('#txtWLWALLETNO').val() == null) {
            $("#formWLUSER .errorss").text('Wallet No. Required');
            return false;
        }
        if ($('#txtWLTranstionNO').val() == '' || $('#txtWLTranstionNO').val() == null) {
            $("#formWLUSER .errorss").text('Transtion No.');
            return false;
        }
        if ($('#txtWLAMOUNT').val() == '' || $('#txtWLAMOUNT').val() == null) {
            $("#formWLUSER .errorss").text('Amount Required');
            return false;
        }
        if ($('#txtWLCOMMENTS').val() == '' || $('#txtWLCOMMENTS').val() == null) {
            $('#txtWLCOMMENTS').val('No Comments')
        }
    }
    else if ($('#ddlWLPaymentModel').val() == 5) {

        if ($('#txtWLAMOUNT').val() == '' || $('#txtWLAMOUNT').val() == null) {
            $("#formWLUSER .errorss").text('Amount Required');
            return false;
        }
        if ($('#txtWLCOMMENTS').val() == '' || $('#txtWLCOMMENTS').val() == null) {
            $('#txtWLCOMMENTS').val('No Comments')
        }

    }
    else if ($('#ddlWLPaymentModel').val() == 6) {

        if ($('#txtWLSETTLEMENTTYPE').val() == '' || $('#txtWLSETTLEMENTTYPE').val() == null) {
            $("#formWLUSER .errorss").text('Settlement Type');
            return false;
        }
        if ($('#txtWLAMOUNT').val() == '' || $('#txtWLAMOUNT').val() == null) {
            $("#formWLUSER .errorss").text('Amount Required');
            return false;
        }
        if ($('#txtWLCOMMENTS').val() == '' || $('#txtWLCOMMENTS').val() == null) {
            $('#txtWLCOMMENTS').val('No Comments');
        }
    }
    else if ($('#ddlWLPaymentModel').val() == 7) {

        if ($('#txtWLCREDITDETAILS').val() == '' || $('#txtWLCREDITDETAILS').val() == null) {
            $("#formWLUSER .errorss").text('Credit Detail Required');
            return false;
        }
        if ($('#txtWLAMOUNT').val() == '' || $('#txtWLAMOUNT').val() == null) {
            $("#formWLUSER .errorss").text('Amount Required');
            return false;
        }
        if ($('#txtWLCOMMENTS').val() == '' || $('#txtWLCOMMENTS').val() == null) {
            $('#txtWLCOMMENTS').val('No Comments');
        }
    }
    else if ($('#ddlWLPaymentModel').val() == 8) {

        if ($('#txtWLREASON').val() == '' || $('#txtWLREASON').val() == null) {
            $("#formWLUSER .errorss").text('Enter Debit Reason');
            return false;
        }
        if ($('#txtWLAMOUNT').val() == '' || $('#txtWLAMOUNT').val() == null) {
            $("#formWLUSER .errorss").text('Amount Required');
            return false;
        }
        if ($('#txtWLCOMMENTS').val() == '' || $('#txtWLCOMMENTS').val() == null) {
            $('#txtWLCOMMENTS').val('No Comments');
        }
    }
    else if ($('#ddlWLPaymentModel').val() == 9) {

        if ($('#txtWLREASON').val() == '' || $('#txtWLREASON').val() == null) {
            $("#formWLUSER .errorss").text('Enter Debit Reason');
            return false;
        }
        if ($('#txtWLAMOUNT').val() == '' || $('#txtWLAMOUNT').val() == null) {
            $("#formWLUSER .errorss").text('Amount Required');
            return false;
        }
        if ($('#txtWLCOMMENTS').val() == '' || $('#txtWLCOMMENTS').val() == null) {
            $('#txtWLCOMMENTS').val('No Comments');
        }
    }

    var name = $("#WLUSER option:selected");
    var idst = $("#ddlWLPaymentModel option:selected").text();
    var collper = $('#txtWLCollectionBy').val();
    var amount = $('#txtWLAMOUNT').val();

    var comment = $('#txtWLCOMMENTS').val();
    var bankname = $("#WLRIbankid option:selected").text();
    var depositeslip = $('#txtWLDEPOSITESLIPNO').val();
    var tranftype = $("#WLTransferType option:selected");
    var accountno = $('#txtWLACCOUNTNO').val();
    var UTRnos = $('#txtWLUTRNO').val();
    var walletname = $("#ddlWLwalletname option:selected");
    var walletno = $('#txtWLWALLETNO').val();
    var transectionno = $('#txtWLTranstionNO').val();
    var setteltype = $('#txtWLSETTLEMENTTYPE').val();
    var crdetails = $('#txtWLCREDITDETAILS').val();
    var sbject = $('#txtWLREASON').val();
    var accnosele = $('#ddlWLwalletname').val();
    $('#WL-name').text(name.text())



    var thtm = '<div id="hdidsss">';
    thtm += '<input id="hdSuperstokistID" name="hdSuperstokistID" type="hidden" name="classid" value="' + $('#WLUSER').val() + '" />';
    thtm += '<input id="hdPaymentMode" name="hdPaymentMode" type="hidden"  value="' + idst + '" />';
    //  thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' +amount + '" />';

    var thtmladminwhitelabeltable = '<table class="table table-responsive " border="1">';
    thtmladminwhitelabeltable += '<tr>';
    thtmladminwhitelabeltable += '<th>Transtion ID </th>';
    thtmladminwhitelabeltable += '<th><label type="text" name="lblAdminwhitelabeltransferid" id="lblAdminwhitelabeltransferid" text=""/></th>';
    thtmladminwhitelabeltable += '</tr>';
    thtmladminwhitelabeltable += '<tr>';
    thtmladminwhitelabeltable += '<th>Whitelabel Firm Name </th>';
    thtmladminwhitelabeltable += '<th>' + name.text() + '</th>';
    thtmladminwhitelabeltable += '</tr>';
    thtmladminwhitelabeltable += '<tr>';
    thtmladminwhitelabeltable += '<th>Payment Mode </th>';
    thtmladminwhitelabeltable += '<th>' + idst + '</th>';
    thtmladminwhitelabeltable += '</tr>';



    if ($("#ddlWLPaymentModel option:selected").index() === 0) {

        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Collection By </th>';
        thtmladminwhitelabeltable += '<th>' + collper + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Amount </th>';
        thtmladminwhitelabeltable += '<th>' + amount + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Comment </th>';
        thtmladminwhitelabeltable += '<th>' + comment + '</th>';
        thtmladminwhitelabeltable += '</tr>';

        thtm += '<input id="hdMDcollection" name="hdMDcollection" type="hidden"  value="' + collper + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#ddlWLPaymentModel option:selected").index() === 1) {
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Bank Name </th>';
        thtmladminwhitelabeltable += '<th>' + bankname + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Amount </th>';
        thtmladminwhitelabeltable += '<th>' + amount + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Deposite Slip No </th>';
        thtmladminwhitelabeltable += '<th>' + depositeslip + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Comment </th>';
        thtmladminwhitelabeltable += '<th>' + comment + '</th>';
        thtmladminwhitelabeltable += '</tr>';

        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accnosele + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDDepositeSlipNo" name="hdMDDepositeSlipNo" type="hidden"  value="' + depositeslip + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#ddlWLPaymentModel option:selected").index() === 2) {


        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>TransferType</th>';
        thtmladminwhitelabeltable += '<th>' + tranftype.text() + '</th>';
        thtmladminwhitelabeltable += '</tr>';

        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Bank Name </th>';
        thtmladminwhitelabeltable += '<th>' + bankname + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Account No </th>';
        thtmladminwhitelabeltable += '<th>' + accountno + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>UTR NO </th>';
        thtmladminwhitelabeltable += '<th>' + UTRnos + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Amount </th>';
        thtmladminwhitelabeltable += '<th>' + amount + '</th>';
        thtmladminwhitelabeltable += '</tr>';


        thtm += '<input id="hdMDTransferType" name="hdMDTransferType" type="hidden"  value="' + tranftype.text() + '" />';
        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accountno + '" />';
        thtm += '<input id="hdMDutrno" name="hdMDutrno" type="hidden"  value="' + UTRnos + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#ddlWLPaymentModel option:selected").index() === 3) {

        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Wallet Name</th>';
        thtmladminwhitelabeltable += '<th>' + walletname.text() + '</th>';
        thtmladminwhitelabeltable += '</tr>';

        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Wallet-No </th>';
        thtmladminwhitelabeltable += '<th>' + walletno + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Transation-No </th>';
        thtmladminwhitelabeltable += '<th>' + transectionno + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Amount </th>';
        thtmladminwhitelabeltable += '<th>' + amount + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Comment </th>';
        thtmladminwhitelabeltable += '<th>' + comment + '</th>';
        thtmladminwhitelabeltable += '</tr>';

        thtm += '<input id="hdMDwallet" name="hdMDwallet" type="hidden"  value="' + walletname.text() + '" />';
        thtm += '<input id="hdMDwalletno" name="hdMDwalletno" type="hidden"  value="' + walletno + '" />';
        thtm += '<input id="hdMDtransationno" name="hdMDtransationno" type="hidden"  value="' + transectionno + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#ddlWLPaymentModel option:selected").index() === 4) {
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Amount </th>';
        thtmladminwhitelabeltable += '<th>' + amount + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Comment </th>';
        thtmladminwhitelabeltable += '<th>' + comment + '</th>';
        thtmladminwhitelabeltable += '</tr>';

        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#ddlWLPaymentModel option:selected").index() === 5) {
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Settelment </th>';
        thtmladminwhitelabeltable += '<th>' + setteltype + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Amount </th>';
        thtmladminwhitelabeltable += '<th>' + amount + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Comment </th>';
        thtmladminwhitelabeltable += '<th>' + comment + '</th>';
        thtmladminwhitelabeltable += '</tr>';


        thtm += '<input id="hdMDsettelment" name="hdMDsettelment" type="hidden"  value="' + setteltype + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#ddlWLPaymentModel option:selected").index() === 6) {
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>CreditDetail </th>';
        thtmladminwhitelabeltable += '<th>' + crdetails + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Amount </th>';
        thtmladminwhitelabeltable += '<th>' + amount + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Comment </th>';
        thtmladminwhitelabeltable += '<th>' + comment + '</th>';
        thtmladminwhitelabeltable += '</tr>';

        thtm += '<input id="hdMDCreditDetail" name="hdMDCreditDetail" type="hidden"  value="' + crdetails + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';

    }
    if ($("#ddlWLPaymentModel option:selected").index() === 7 || $("#ddlWLPaymentModel option:selected").index() === 8) {
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Subject </th>';
        thtmladminwhitelabeltable += '<th>' + sbject + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Amount </th>';
        thtmladminwhitelabeltable += '<th>' + amount + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtmladminwhitelabeltable += '<tr>';
        thtmladminwhitelabeltable += '<th>Comment </th>';
        thtmladminwhitelabeltable += '<th>' + comment + '</th>';
        thtmladminwhitelabeltable += '</tr>';
        thtm += '<input id="hdMDsubject" name="hdMDsubject" type="hidden"  value="' + sbject + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    thtm += '</div>';

    $.ajax({
        url: callurl,
        success: function (data) {
            $('#lblAdminwhitelabeltransferid').html('<b>' + data + '</b>')

        }

    })

    thtmladminwhitelabeltable += '</table>';
    $('#tblshowinformadmintoWebapi').html(thtmladminwhitelabeltable);

    $('#formWLUSER').hide();
    $('#WLUSERshowinformation').show();

    $('.txtcodecodec').val('');
    //   $('#txtcode').val('')

    $('#hdWLninputvalues').html(thtm);



})

///APIEND








////////////////






///API Start

$('#btnAPIUSERInform').click(function () {

    var callurl = $('#btnAPIUSERInform').attr('data-buttoncall')
    /*   alert(callurl)*/
    $('#hdRETninputvalues').empty();
    $('#tblshowinformadmintoAPI').empty();
    $('#txtAPICollectionBy').val('')

    if ($('#APIUSER').val() == '' || $('#APIUSER').val() == null) {
        $(".errorss").text('Select Api User');
        return false;
    }
    if ($('#ddlAPIPaymentModel').val() == '' || $('#ddlAPIPaymentModel').val() == null) {
        $(".errorss").text('Pay mode REQUIRED');
        return false;
    }
    if ($('#ddlAPIPaymentModel').val() == 1) {
        if ($('#txtAPICollectionBy').val() == '' || $('#txtAPICollectionBy').val() == null) {
            $('#txtAPICollectionBy').val('Self')
        }
        if ($('#txtAPIAMOUNT').val() == '' || $('#txtAPIAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtAPICOMMENTS').val() == '' || $('#txtAPICOMMENTS').val() == null) {
            $('#txtAPICOMMENTS').val('No Comment')
        }


    }
    else if ($('#ddlAPIPaymentModel').val() == 2) {

        if ($('#APIRIbankid').val() == '' || $('#APIRIbankid').val() == null) {
            $(".errorss").text('Select Bank Name');
            return false;
        }
        if ($('#txtAPIAMOUNT').val() == '' || $('#txtAPIAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtAPIDEPOSITESLIPNO').val() == '' || $('#txtAPIDEPOSITESLIPNO').val() == null) {
            $(".errorss").text('Enter Deposite Slip No.');
            return false;
        }

        if ($('#txtAPICOMMENTS').val() == '' || $('#txtAPICOMMENTS').val() == null) {
            $('#txtAPICOMMENTS').val('No Comment')
        }

    }

    else if ($('#ddlAPIPaymentModel').val() == 3) {

        if ($('#APITransferType').val() == '' || $('#APITransferType').val() == null) {
            $(".errorss").text('Select Transfer Type');
            return false;
        }
        if ($('#APIRIbankid').val() == '' || $('#APIRIbankid').val() == null) {
            $(".errorss").text('Select Bank Name');
            return false;
        }
        if ($('#txtAPIACCOUNTNO').val() == '' || $('#txtAPIACCOUNTNO').val() == null) {
            $(".errorss").text('Account No Required');
            return false;
        }
        if ($('#txtAPIUTRNO').val() == '' || $('#txtAPIUTRNO').val() == null) {
            $(".errorss").text('UTR No Required');
            return false;
        }
        if ($('#txtAPIAMOUNT').val() == '' || $('#txtAPIAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }

    }
    else if ($('#ddlAPIPaymentModel').val() == 4) {

        if ($('#ddlAPIwalletname').val() == '' || $('#ddlAPIwalletname').val() == null) {
            $(".errorss").text('Select Walet Name');
            return false;
        }
        if ($('#txtAPIWALLETNO').val() == '' || $('#txtAPIWALLETNO').val() == null) {
            $(".errorss").text('Wallet No. Required');
            return false;
        }
        if ($('#txtAPITranstionNO').val() == '' || $('#txtAPITranstionNO').val() == null) {
            $(".errorss").text('Transtion No.');
            return false;
        }
        if ($('#txtAPIAMOUNT').val() == '' || $('#txtAPIAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtAPICOMMENTS').val() == '' || $('#txtAPICOMMENTS').val() == null) {
            $('#txtAPICOMMENTS').val('No Comment')
        }
    }
    else if ($('#ddlAPIPaymentModel').val() == 5) {

        if ($('#txtAPIAMOUNT').val() == '' || $('#txtAPIAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtAPICOMMENTS').val() == '' || $('#txtAPICOMMENTS').val() == null) {
            $('#txtAPICOMMENTS').val('No Comment')
        }

    }
    else if ($('#ddlAPIPaymentModel').val() == 6) {

        if ($('#txtAPISETTLEMENTTYPE').val() == '' || $('#txtAPISETTLEMENTTYPE').val() == null) {
            $(".errorss").text('Settlement Type');
            return false;
        }
        if ($('#txtAPIAMOUNT').val() == '' || $('#txtAPIAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtAPICOMMENTS').val() == '' || $('#txtAPICOMMENTS').val() == null) {
            $('#txtAPICOMMENTS').val('No Comment')
        }
    }
    else if ($('#ddlAPIPaymentModel').val() == 7) {

        if ($('#txtAPICREDITDETAILS').val() == '' || $('#txtAPICREDITDETAILS').val() == null) {
            $(".errorss").text('Credit Detail Required');
            return false;
        }
        if ($('#txtAPIAMOUNT').val() == '' || $('#txtAPIAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtAPICOMMENTS').val() == '' || $('#txtAPICOMMENTS').val() == null) {
            $('#txtAPICOMMENTS').val('No Comment')
        }
    }
    else if ($('#ddlAPIPaymentModel').val() == 8) {

        if ($('#txtAPIREASON').val() == '' || $('#txtAPIREASON').val() == null) {
            $(".errorss").text('Enter Debit Reason');
            return false;
        }
        if ($('#txtAPIAMOUNT').val() == '' || $('#txtAPIAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtAPICOMMENTS').val() == '' || $('#txtAPICOMMENTS').val() == null) {
            $('#txtAPICOMMENTS').val('No Comment')
        }
    }
    else if ($('#ddlAPIPaymentModel').val() == 9) {

        if ($('#txtAPIREASON').val() == '' || $('#txtAPIREASON').val() == null) {
            $(".errorss").text('Enter Debit Reason');
            return false;
        }
        if ($('#txtAPIAMOUNT').val() == '' || $('#txtAPIAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtAPICOMMENTS').val() == '' || $('#txtAPICOMMENTS').val() == null) {
            $('#txtAPICOMMENTS').val('No Comment')
        }
    }

    var name = $("#APIUSER option:selected");
    var idst = $("#ddlAPIPaymentModel option:selected").text();
    var collper = $('#txtAPICollectionBy').val();
    var amount = $('#txtAPIAMOUNT').val();

    var comment = $('#txtAPICOMMENTS').val();
    var bankname = $("#APIRIbankid option:selected").text();
    var depositeslip = $('#txtAPIDEPOSITESLIPNO').val();
    var tranftype = $("#APITransferType option:selected");
    var accountno = $('#txtAPIACCOUNTNO').val();
    var UTRnos = $('#txtAPIUTRNO').val();
    var walletname = $("#ddlAPIwalletname option:selected");
    var walletno = $('#txtAPIWALLETNO').val();
    var transectionno = $('#txtAPITranstionNO').val();
    var setteltype = $('#txtAPISETTLEMENTTYPE').val();
    var crdetails = $('#txtAPICREDITDETAILS').val();
    var sbject = $('#txtAPIREASON').val();
    var accnosele = $('#APIRIbankid').val();
    $('#API-name').text(name.text())


    var thtmadminapiltable = '<table class="table table-responsive " border="1">';

    thtmadminapiltable += '<tr>';
    thtmadminapiltable += '<th>Transtion ID </th>';
    thtmadminapiltable += '<th><label type="text" name="lblAdminAPItransferid" id="lblAdminAPItransferid" text=""/></th>';
    thtmadminapiltable += '</tr>';
    thtmadminapiltable += '<tr>';
    thtmadminapiltable += '<th>MD Firm Name </th>';
    thtmadminapiltable += '<th>' + name.text() + '</th>';
    thtmadminapiltable += '</tr>';
    thtmadminapiltable += '<tr>';
    thtmadminapiltable += '<th>Payment Mode </th>';
    thtmadminapiltable += '<th>' + idst + '</th>';
    thtmadminapiltable += '</tr>';



    var thtm = '<div id="hdidsss">';
    thtm += '<input id="hdSuperstokistID" name="hdSuperstokistID" type="hidden" name="classid" value="' + $('#APIUSER').val() + '" />';
    thtm += '<input id="hdPaymentMode" name="hdPaymentMode" type="hidden"  value="' + idst + '" />';

    //$('#API-name').text(name.text())
    //$('#API-Mode').text(idst)

    if ($("#ddlAPIPaymentModel option:selected").index() === 0) {

        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Collection By </th>';
        thtmadminapiltable += '<th>' + collper + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Amount </th>';
        thtmadminapiltable += '<th>' + amount + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Comment </th>';
        thtmadminapiltable += '<th>' + comment + '</th>';
        thtmadminapiltable += '</tr>';



        thtm += '<input id="hdMDcollection" name="hdMDcollection" type="hidden"  value="' + collper + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#ddlAPIPaymentModel option:selected").index() === 1) {


        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Bank Name </th>';
        thtmadminapiltable += '<th>' + bankname + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Amount </th>';
        thtmadminapiltable += '<th>' + amount + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Deposite Slip No </th>';
        thtmadminapiltable += '<th>' + depositeslip + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Comment </th>';
        thtmadminapiltable += '<th>' + comment + '</th>';
        thtmadminapiltable += '</tr>';



        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accnosele + '" />';
        thtm += '<input id="hdMDDepositeSlipNo" name="hdMDDepositeSlipNo" type="hidden"  value="' + depositeslip + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#ddlAPIPaymentModel option:selected").index() === 2) {

        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>TransferType</th>';
        thtmadminapiltable += '<th>' + tranftype.text() + '</th>';
        thtmadminapiltable += '</tr>';

        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Bank Name </th>';
        thtmadminapiltable += '<th>' + bankname + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Account No </th>';
        thtmadminapiltable += '<th>' + accountno + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>UTR NO </th>';
        thtmadminapiltable += '<th>' + UTRnos + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Amount </th>';
        thtmadminapiltable += '<th>' + amount + '</th>';
        thtmadminapiltable += '</tr>';


        thtm += '<input id="hdMDTransferType" name="hdMDTransferType" type="hidden"  value="' + tranftype.text() + '" />';
        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accountno + '" />';
        thtm += '<input id="hdMDutrno" name="hdMDutrno" type="hidden"  value="' + UTRnos + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#ddlAPIPaymentModel option:selected").index() === 3) {

        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Wallet Name</th>';
        thtmadminapiltable += '<th>' + walletname.text() + '</th>';
        thtmadminapiltable += '</tr>';

        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Wallet-No </th>';
        thtmadminapiltable += '<th>' + walletno + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Transation-No </th>';
        thtmadminapiltable += '<th>' + transectionno + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Amount </th>';
        thtmadminapiltable += '<th>' + amount + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Comment </th>';
        thtmadminapiltable += '<th>' + comment + '</th>';
        thtmadminapiltable += '</tr>';

        thtm += '<input id="hdMDwallet" name="hdMDwallet" type="hidden"  value="' + walletname.text() + '" />';
        thtm += '<input id="hdMDwalletno" name="hdMDwalletno" type="hidden"  value="' + walletno + '" />';
        thtm += '<input id="hdMDtransationno" name="hdMDtransationno" type="hidden"  value="' + transectionno + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#ddlAPIPaymentModel option:selected").index() === 4) {


        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Amount </th>';
        thtmadminapiltable += '<th>' + amount + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Comment </th>';
        thtmadminapiltable += '<th>' + comment + '</th>';
        thtmadminapiltable += '</tr>';



        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#ddlAPIPaymentModel option:selected").index() === 5) {

        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Settelment </th>';
        thtmadminapiltable += '<th>' + setteltype + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Amount </th>';
        thtmadminapiltable += '<th>' + amount + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Comment </th>';
        thtmadminapiltable += '<th>' + comment + '</th>';
        thtmadminapiltable += '</tr>';

        thtm += '<input id="hdMDsettelment" name="hdMDsettelment" type="hidden"  value="' + setteltype + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#ddlAPIPaymentModel option:selected").index() === 6) {
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>CreditDetail </th>';
        thtmadminapiltable += '<th>' + crdetails + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Amount </th>';
        thtmadminapiltable += '<th>' + amount + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Comment </th>';
        thtmadminapiltable += '<th>' + comment + '</th>';
        thtmadminapiltable += '</tr>';


        thtm += '<input id="hdMDCreditDetail" name="hdMDCreditDetail" type="hidden"  value="' + crdetails + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';

    }
    if ($("#ddlAPIPaymentModel option:selected").index() === 7 || $("#ddlAPIPaymentModel option:selected").index() === 8) {


        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Subject </th>';
        thtmadminapiltable += '<th>' + sbject + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Amount </th>';
        thtmadminapiltable += '<th>' + amount + '</th>';
        thtmadminapiltable += '</tr>';
        thtmadminapiltable += '<tr>';
        thtmadminapiltable += '<th>Comment </th>';
        thtmadminapiltable += '<th>' + comment + '</th>';
        thtmadminapiltable += '</tr>';


        thtm += '<input id="hdMDsubject" name="hdMDsubject" type="hidden"  value="' + sbject + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    thtm += '</div>';

    $.ajax({
        url: callurl,
        success: function (data) {
            $('#lblAdminAPItransferid').html('<b>' + data + '</b>')

        }

    })

    thtmadminapiltable += '</table>';

    $('#tblshowinformadmintoAPI').html(thtmadminapiltable);

    $('#formAPIUSER').hide();
    $('#APIUSERshowinformation').show();
    $('.txtcodecodec').val('');
    //   $('#txtcode').val('')

    $('#hdAPIninputvalues').html(thtm);



})

///APIEND




///Retailer Start
$('#txtRETAMOUNT,#txtsupramount,#txtDISTAMOUNT,#txtAPIAMOUNT,#txtWLAMOUNT').keypress(function (e) {
    if (this.value.length == 0 && e.which == 48) {
        return false;
    }
});
$('#btnRetailerInform').click(function () {

    var callurl = $('#btnRetailerInform').attr('data-buttoncall')
    /* alert(callurl)*/
    $('#txtRETCollectionBy').val('')
    $('#hdRETninputvalues').empty();
    $('#tblshowinformadmintoRetailer').empty();

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
            $('#txtRETCollectionBy').val('Self')
        }
        if ($('#txtRETAMOUNT').val() == '' || $('#txtRETAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtRETCOMMENTS').val() == '' || $('#txtRETCOMMENTS').val() == null) {
            $('#txtRETCOMMENTS').val('No Comment')
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
            $('#txtRETCOMMENTS').val('No Comment')
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
            $('#txtRETCOMMENTS').val('No Comment')
        }
    }
    else if ($('#ddlRETPaymentModel').val() == 5) {

        if ($('#txtRETAMOUNT').val() == '' || $('#txtRETAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtRETCOMMENTS').val() == '' || $('#txtRETCOMMENTS').val() == null) {
            $('#txtRETCOMMENTS').val('No Comment')
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
            $('#txtRETCOMMENTS').val('No Comment')
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
            $('#txtRETCOMMENTS').val('No Comment')
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
            $('#txtRETCOMMENTS').val('No Comment')
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
            $('#txtRETCOMMENTS').val('No Comment')
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
    //$('#d-name').text(name.text())



    var thtmltableradmintorem = '<table class="table table-responsive " border="1">';

    thtmltableradmintorem += '<tr>';
    thtmltableradmintorem += '<th>Transtion ID </th>';
    thtmltableradmintorem += '<th><label type="text" name="lbladminremtransferid" id="lbladminremtransferid" text=""/></th>';
    thtmltableradmintorem += '</tr>';
    thtmltableradmintorem += '<tr>';
    thtmltableradmintorem += '<th>Retailer Firm Name </th>';
    thtmltableradmintorem += '<th>' + name.text() + '</th>';
    thtmltableradmintorem += '</tr>';
    thtmltableradmintorem += '<tr>';
    thtmltableradmintorem += '<th>Payment Mode </th>';
    thtmltableradmintorem += '<th>' + idst + '</th>';
    thtmltableradmintorem += '</tr>';

    var thtm = '<div id="hdidsss">';
    thtm += '<input id="hdSuperstokistID" name="hdSuperstokistID" type="hidden" name="classid" value="' + $('#RLMID').val() + '" />';
    thtm += '<input id="hdPaymentMode" name="hdPaymentMode" type="hidden"  value="' + idst + '" />';
    //  thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' +amount + '" />';

    //$('#RR-name').text(name.text())
    //$('#RR-Mode').text(idst)

    if ($("#ddlRETPaymentModel option:selected").index() === 0) {
        //$('#RR-collection').parent().show();
        //$('#RR-ammount').parent().show();
        //$('#RR-comment').parent().show();

        //$('#RR-collection').text(collper)
        //$('#RR-ammount').text(amount)
        //$('#RR-comment').text(comment)

        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Collection By </th>';
        thtmltableradmintorem += '<th>' + collper + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Amount </th>';
        thtmltableradmintorem += '<th>' + amount + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Comment </th>';
        thtmltableradmintorem += '<th>' + comment + '</th>';
        thtmltableradmintorem += '</tr>';


        thtm += '<input id="hdMDcollection" name="hdMDcollection" type="hidden"  value="' + collper + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#ddlRETPaymentModel option:selected").index() === 1) {
        //$('#RR-Bank').parent().show();
        //$('#RR-ammount').parent().show();
        //$('#RR-DepositeSlipNo').parent().show();
        //$('#RR-comment').parent().show();


        //$('#RR-Bank').text(bankname)
        //$('#RR-ammount').text(amount)
        //$('#RR-DepositeSlipNo').text(depositeslip);
        //$('#RR-comment').text(comment)

        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Bank Name </th>';
        thtmltableradmintorem += '<th>' + bankname + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Amount </th>';
        thtmltableradmintorem += '<th>' + amount + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Deposite Slip No </th>';
        thtmltableradmintorem += '<th>' + depositeslip + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Comment </th>';
        thtmltableradmintorem += '<th>' + comment + '</th>';
        thtmltableradmintorem += '</tr>';

        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accnosele + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDDepositeSlipNo" name="hdMDDepositeSlipNo" type="hidden"  value="' + depositeslip + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#ddlRETPaymentModel option:selected").index() === 2) {
        //$('#RR-TransferType').parent().show();
        //$('#RR-Bank').parent().show();
        //$('#RR-ammount').parent().show();
        //$('#RR-account-no').parent().show();
        //$('#RR-utr-no').parent().show();

        //$('#RR-TransferType').text(tranftype.text());
        //$('#RR-Bank').text(bankname)
        //$('#RR-account-no').text(accountno);
        //$('#RR-utr-no').text(UTRnos)
        //$('#RR-ammount').text(amount)

        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>TransferType</th>';
        thtmltableradmintorem += '<th>' + tranftype.text() + '</th>';
        thtmltableradmintorem += '</tr>';

        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Bank Name </th>';
        thtmltableradmintorem += '<th>' + bankname + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Account No </th>';
        thtmltableradmintorem += '<th>' + accountno + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>UTR NO </th>';
        thtmltableradmintorem += '<th>' + UTRnos + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Amount </th>';
        thtmltableradmintorem += '<th>' + amount + '</th>';
        thtmltableradmintorem += '</tr>';




        thtm += '<input id="hdMDTransferType" name="hdMDTransferType" type="hidden"  value="' + tranftype.text() + '" />';
        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accountno + '" />';
        thtm += '<input id="hdMDutrno" name="hdMDutrno" type="hidden"  value="' + UTRnos + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#ddlRETPaymentModel option:selected").index() === 3) {
        //$('#RR-wallet').parent().show();
        //$('#RR-wallet-no').parent().show();
        //$('#RR-transation-no').parent().show();
        //$('#RR-ammount').parent().show();
        //$('#RR-comment').parent().show();

        //$('#RR-wallet').text(walletname.text())
        //$('#RR-wallet-no').text(walletno)
        //$('#RR-transation-no').text(transectionno)
        //$('#RR-ammount').text(amount)
        //$('#RR-comment').text(comment)

        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Wallet Name</th>';
        thtmltableradmintorem += '<th>' + walletname.text() + '</th>';
        thtmltableradmintorem += '</tr>';

        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Wallet-No </th>';
        thtmltableradmintorem += '<th>' + walletno + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Transation-No </th>';
        thtmltableradmintorem += '<th>' + transectionno + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Amount </th>';
        thtmltableradmintorem += '<th>' + amount + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Comment </th>';
        thtmltableradmintorem += '<th>' + comment + '</th>';
        thtmltableradmintorem += '</tr>';


        thtm += '<input id="hdMDwallet" name="hdMDwallet" type="hidden"  value="' + walletname.text() + '" />';
        thtm += '<input id="hdMDwalletno" name="hdMDwalletno" type="hidden"  value="' + walletno + '" />';
        thtm += '<input id="hdMDtransationno" name="hdMDtransationno" type="hidden"  value="' + transectionno + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#ddlRETPaymentModel option:selected").index() === 4) {
        //$('#RR-ammount').parent().show();
        //$('#RR-comment').parent().show();

        //$('#RR-ammount').text(amount)
        //$('#RR-comment').text(comment)

        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Amount </th>';
        thtmltableradmintorem += '<th>' + amount + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Comment </th>';
        thtmltableradmintorem += '<th>' + comment + '</th>';
        thtmltableradmintorem += '</tr>';


        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#ddlRETPaymentModel option:selected").index() === 5) {
        //$('#RR-settelment').parent().show();
        //$('#RR-ammount').parent().show();
        //$('#RR-comment').parent().show();

        //$('#RR-settelment').text(setteltype);
        //$('#RR-ammount').text(amount)
        //$('#RR-comment').text(comment)

        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Settelment </th>';
        thtmltableradmintorem += '<th>' + setteltype + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Amount </th>';
        thtmltableradmintorem += '<th>' + amount + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Comment </th>';
        thtmltableradmintorem += '<th>' + comment + '</th>';
        thtmltableradmintorem += '</tr>';


        thtm += '<input id="hdMDsettelment" name="hdMDsettelment" type="hidden"  value="' + setteltype + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#ddlRETPaymentModel option:selected").index() === 6) {
        //$('#RR-CreditDetail').parent().show();
        //$('#RR-ammount').parent().show();
        //$('#RR-comment').parent().show();



        //$('#RR-CreditDetail').text(crdetails)
        //$('#RR-ammount').text(amount)
        //$('#RR-comment').text(comment)

        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>CreditDetail </th>';
        thtmltableradmintorem += '<th>' + crdetails + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Amount </th>';
        thtmltableradmintorem += '<th>' + amount + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Comment </th>';
        thtmltableradmintorem += '<th>' + comment + '</th>';
        thtmltableradmintorem += '</tr>';


        thtm += '<input id="hdMDCreditDetail" name="hdMDCreditDetail" type="hidden"  value="' + crdetails + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';

    }
    if ($("#ddlRETPaymentModel option:selected").index() === 7 || $("#ddlRETPaymentModel option:selected").index() === 8 || $("#ddlRETPaymentModel option:selected").index() === 9) {
        //$('#RR-subject').parent().show();
        //$('#RR-ammount').parent().show();
        //$('#RR-comment').parent().show();


        //$('#RR-subject').text(sbject)
        //$('#RR-ammount').text(amount)
        //$('#RR-comment').text(comment)

        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Subject </th>';
        thtmltableradmintorem += '<th>' + sbject + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Amount </th>';
        thtmltableradmintorem += '<th>' + amount + '</th>';
        thtmltableradmintorem += '</tr>';
        thtmltableradmintorem += '<tr>';
        thtmltableradmintorem += '<th>Comment </th>';
        thtmltableradmintorem += '<th>' + comment + '</th>';
        thtmltableradmintorem += '</tr>';

        thtm += '<input id="hdMDsubject" name="hdMDsubject" type="hidden"  value="' + sbject + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }

    thtm += '</div>';
    $.ajax({
        url: callurl,
        success: function (data) {
            $('#lbladminremtransferid').html('<b>' + data + '</b>')

        }

    })

    thtmltableradmintorem += '</table>';
    $('#tblshowinformadmintoRetailer').html(thtmltableradmintorem);
    $('#hdRETninputvalues').html(thtm);
    $('#formRetailer').hide();
    $('#Retailershowinformation').show();
    $('.txtcodecodec').val('');
    //   $('#txtcode').val('')





})

///Retailer END










///Distreibutort Start

$('#btndistributorInform').click(function () {

    var callurl = $('#btndistributorInform').attr('data-buttoncall')
    /* alert(callurl)*/
    $('#hdddninputvalues').empty();
    $('#tblshowinformadmintodealer').empty();
    $('#txtDISTCollectionBy').val('');
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
            // $(".errorss").text('Collection Person Name');
            // return false;
            $('#txtDISTCollectionBy').val('Self')

        }
        if ($('#txtDISTAMOUNT').val() == '' || $('#txtDISTAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtDISTCOMMENTS').val() == '' || $('#txtDISTCOMMENTS').val() == null) {
            //$(".errorss").text('Comment Required');
            //return false;
            $('#txtDISTCOMMENTS').val('No Comment')
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
            $('#txtDISTCOMMENTS').val('No Comment')
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
            $('#txtDISTCOMMENTS').val('No Comment')
        }
    }
    else if ($('#ddlDISTPaymentModel').val() == 5) {

        if ($('#txtDISTAMOUNT').val() == '' || $('#txtDISTAMOUNT').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtDISTCOMMENTS').val() == '' || $('#txtDISTCOMMENTS').val() == null) {
            $('#txtDISTCOMMENTS').val('No Comment')
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
            $('#txtDISTCOMMENTS').val('No Comment')
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
            $('#txtDISTCOMMENTS').val('No Comment')
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
            $('#txtDISTCOMMENTS').val('No Comment')
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
            $('#txtDISTCOMMENTS').val('No Comment')
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





    var thtmltabledistributor = '<table class="table table-responsive " border="1">';
    thtmltabledistributor += '<tr>';
    thtmltabledistributor += '<th>Transtion ID </th>';
    thtmltabledistributor += '<th><label type="text" name="lbldistributortransferid" id="lbldistributortransferid" text=""/></th>';
    thtmltabledistributor += '</tr>';
    thtmltabledistributor += '<tr>';
    thtmltabledistributor += '<th>Distributor Firm Name </th>';
    thtmltabledistributor += '<th>' + name.text() + '</th>';
    thtmltabledistributor += '</tr>';
    thtmltabledistributor += '<tr>';
    thtmltabledistributor += '<th>Payment Mode </th>';
    thtmltabledistributor += '<th>' + idst + '</th>';
    thtmltabledistributor += '</tr>';
    var transid = '';


    if ($("#ddlDISTPaymentModel option:selected").index() === 0) {
        //$('#d-collection').parent().show();
        //$('#d-ammount').parent().show();
        //$('#d-comment').parent().show();

        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Collection By </th>';
        thtmltabledistributor += '<th>' + collper + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Amount </th>';
        thtmltabledistributor += '<th>' + amount + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Comment </th>';
        thtmltabledistributor += '<th>' + comment + '</th>';
        thtmltabledistributor += '</tr>';


        //$('#d-collection').text(collper)
        //$('#d-ammount').text(amount)
        //$('#d-comment').text(comment)
        thtm += '<input id="hdMDcollection" name="hdMDcollection" type="hidden"  value="' + collper + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#ddlDISTPaymentModel option:selected").index() === 1) {
        //$('#d-Bank').parent().show();
        //$('#d-ammount').parent().show();
        //$('#d-DepositeSlipNo').parent().show();
        //$('#d-comment').parent().show();


        //$('#d-Bank').text(bankname)
        //$('#d-ammount').text(amount)
        //$('#d-DepositeSlipNo').text(depositeslip);
        //$('#d-comment').text(comment)

        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Bank Name </th>';
        thtmltabledistributor += '<th>' + bankname + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Amount </th>';
        thtmltabledistributor += '<th>' + amount + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Deposite Slip No </th>';
        thtmltabledistributor += '<th>' + depositeslip + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Comment </th>';
        thtmltabledistributor += '<th>' + comment + '</th>';
        thtmltabledistributor += '</tr>';

        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accnosele + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDDepositeSlipNo" name="hdMDDepositeSlipNo" type="hidden"  value="' + depositeslip + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#ddlDISTPaymentModel option:selected").index() === 2) {
        //$('#d-TransferType').parent().show();
        //$('#d-Bank').parent().show();
        //$('#d-ammount').parent().show();
        //$('#d-account-no').parent().show();
        //$('#d-utr-no').parent().show();

        //$('#d-TransferType').text(tranftype.text());
        //$('#d-Bank').text(bankname)
        //$('#d-account-no').text(accountno);
        //$('#d-utr-no').text(UTRnos)
        //$('#d-ammount').text(amount)


        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>TransferType</th>';
        thtmltabledistributor += '<th>' + tranftype.text() + '</th>';
        thtmltabledistributor += '</tr>';

        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Bank Name </th>';
        thtmltabledistributor += '<th>' + bankname + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Account No </th>';
        thtmltabledistributor += '<th>' + accountno + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>UTR NO </th>';
        thtmltabledistributor += '<th>' + UTRnos + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Amount </th>';
        thtmltabledistributor += '<th>' + amount + '</th>';
        thtmltabledistributor += '</tr>';





        thtm += '<input id="hdMDTransferType" name="hdMDTransferType" type="hidden"  value="' + tranftype.text() + '" />';
        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accountno + '" />';
        thtm += '<input id="hdMDutrno" name="hdMDutrno" type="hidden"  value="' + UTRnos + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#ddlDISTPaymentModel option:selected").index() === 3) {



        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Wallet Name</th>';
        thtmltabledistributor += '<th>' + walletname.text() + '</th>';
        thtmltabledistributor += '</tr>';

        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Wallet-No </th>';
        thtmltabledistributor += '<th>' + walletno + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Transation-No </th>';
        thtmltabledistributor += '<th>' + transectionno + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Amount </th>';
        thtmltabledistributor += '<th>' + amount + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Comment </th>';
        thtmltabledistributor += '<th>' + comment + '</th>';
        thtmltabledistributor += '</tr>';


        thtm += '<input id="hdMDwallet" name="hdMDwallet" type="hidden"  value="' + walletname.text() + '" />';
        thtm += '<input id="hdMDwalletno" name="hdMDwalletno" type="hidden"  value="' + walletno + '" />';
        thtm += '<input id="hdMDtransationno" name="hdMDtransationno" type="hidden"  value="' + transectionno + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#ddlDISTPaymentModel option:selected").index() === 4) {
        //$('#d-ammount').parent().show();
        //$('#d-comment').parent().show();

        //$('#d-ammount').text(amount)
        //$('#d-comment').text(comment)

        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Amount </th>';
        thtmltabledistributor += '<th>' + amount + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Comment </th>';
        thtmltabledistributor += '<th>' + comment + '</th>';
        thtmltabledistributor += '</tr>';

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

        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Settelment </th>';
        thtmltabledistributor += '<th>' + setteltype + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Amount </th>';
        thtmltabledistributor += '<th>' + amount + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Comment </th>';
        thtmltabledistributor += '<th>' + comment + '</th>';
        thtmltabledistributor += '</tr>';



        thtm += '<input id="hdMDsettelment" name="hdMDsettelment" type="hidden"  value="' + setteltype + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#ddlDISTPaymentModel option:selected").index() === 6) {
        //$('#d-CreditDetail').parent().show();
        //$('#d-ammount').parent().show();
        //$('#d-comment').parent().show();



        //$('#d-CreditDetail').text(crdetails)
        //$('#d-ammount').text(amount)
        //$('#d-comment').text(comment)

        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>CreditDetail </th>';
        thtmltabledistributor += '<th>' + crdetails + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Amount </th>';
        thtmltabledistributor += '<th>' + amount + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Comment </th>';
        thtmltabledistributor += '<th>' + comment + '</th>';
        thtmltabledistributor += '</tr>';

        thtm += '<input id="hdMDCreditDetail" name="hdMDCreditDetail" type="hidden"  value="' + crdetails + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';

    }
    if ($("#ddlDISTPaymentModel option:selected").index() === 7 || $("#ddlDISTPaymentModel option:selected").index() === 8) {



        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Subject </th>';
        thtmltabledistributor += '<th>' + sbject + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Amount </th>';
        thtmltabledistributor += '<th>' + amount + '</th>';
        thtmltabledistributor += '</tr>';
        thtmltabledistributor += '<tr>';
        thtmltabledistributor += '<th>Comment </th>';
        thtmltabledistributor += '<th>' + comment + '</th>';
        thtmltabledistributor += '</tr>';

        thtm += '<input id="hdMDsubject" name="hdMDsubject" type="hidden"  value="' + sbject + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    thtm += '</div>';

    $.ajax({
        url: callurl,
        success: function (data) {
            $('#lbldistributortransferid').html('<b>' + data + '</b>')

        }

    })
    thtmltabledistributor += '</table>';
    $('#tblshowinformadmintodealer').html(thtmltabledistributor);
    $('#frmdata1').hide();
    $('.txtcodecodec').val('');
    // $('#txtcode').val('')
    $('#hdddninputvalues').html(thtm);



})

///Distreibutort END

///Super TockList Start


$('#btnshowdetails').click(function () {

    var callurl = $('#btnshowdetails').attr('data-buttoncall')
    /*   alert(callurl)*/
    $('#hdninputvalues').empty();
    $('#tblshowinform').empty();
    $('#txtsuprcollperson').val('');
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
            //$(".errorss").text('Collection Person Name');
            //return false;

            $('#txtsuprcollperson').val('Self');
        }
        if ($('#txtsupramount').val() == '' || $('#txtsupramount').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtsuprarea').val() == '' || $('#txtsuprarea').val() == null) {
            //$(".errorss").text('Comment Required');
            //return false;
            $('#txtsuprarea').val('self');
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
            $('#txtsuprarea').val('No Comments');
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
            $('#txtsuprarea').val('No Comments');
        }
    }
    else if ($('#idst').val() == 5) {

        if ($('#txtsupramount').val() == '' || $('#txtsupramount').val() == null) {
            $(".errorss").text('Amount Required');
            return false;
        }
        if ($('#txtsuprarea').val() == '' || $('#txtsuprarea').val() == null) {
            $('#txtsuprarea').val('No Comments');
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
            $('#txtsuprarea').val('No Comments');
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
            $('#txtsuprarea').val('No Comments');
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
            $('#txtsuprarea').val('No Comments');
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

            $('#txtsuprarea').val('No Comments');
        }
    }



    $('label').next('.est').remove()
    var thtmltable = '<table class="table table-responsive " border="1">';

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

    var thtm = '<div id="hdidsss">';
    thtm += '<input id="hdSuperstokistID" name="hdSuperstokistID" type="hidden" name="classid" value="' + $('#SuperstokistID').val() + '" />';
    thtm += '<input id="hdPaymentMode" name="hdPaymentMode" type="hidden"  value="' + idst + '" />';


    thtmltable += '<tr>';
    thtmltable += '<th>Transtion ID </th>';
    thtmltable += '<th><label type="text" name="lbltransferid" id="lbltransferid" text=""/></th>';
    thtmltable += '</tr>';
    thtmltable += '<tr>';
    thtmltable += '<th>MD Firm Name </th>';
    thtmltable += '<th>' + name.text() + '</th>';
    thtmltable += '</tr>';
    thtmltable += '<tr>';
    thtmltable += '<th>Payment Mode </th>';
    thtmltable += '<th>' + idst + '</th>';
    thtmltable += '</tr>';


    var transid = '';



    if ($("#idst option:selected").index() === 0) {

        thtmltable += '<tr>';
        thtmltable += '<th>Collection By </th>';
        thtmltable += '<th>' + collper + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Amount </th>';
        thtmltable += '<th>' + amount + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Comment </th>';
        thtmltable += '<th>' + comment + '</th>';
        thtmltable += '</tr>';





        thtm += '<input id="hdMDcollection" name="hdMDcollection" type="hidden"  value="' + collper + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#idst option:selected").index() === 1) {

        thtmltable += '<tr>';
        thtmltable += '<th>Bank Name </th>';
        thtmltable += '<th>' + bankname + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Amount </th>';
        thtmltable += '<th>' + amount + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Deposite Slip No </th>';
        thtmltable += '<th>' + depositeslip + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Comment </th>';
        thtmltable += '<th>' + comment + '</th>';
        thtmltable += '</tr>';





        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accnosele + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDDepositeSlipNo" name="hdMDDepositeSlipNo" type="hidden"  value="' + depositeslip + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#idst option:selected").index() === 2) {


        thtmltable += '<tr>';
        thtmltable += '<th>TransferType</th>';
        thtmltable += '<th>' + tranftype.text() + '</th>';
        thtmltable += '</tr>';

        thtmltable += '<tr>';
        thtmltable += '<th>Bank Name </th>';
        thtmltable += '<th>' + bankname + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Account No </th>';
        thtmltable += '<th>' + accountno + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>UTR NO </th>';
        thtmltable += '<th>' + UTRnos + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Amount </th>';
        thtmltable += '<th>' + amount + '</th>';
        thtmltable += '</tr>';


        thtm += '<input id="hdMDTransferType" name="hdMDTransferType" type="hidden"  value="' + tranftype.text() + '" />';
        thtm += '<input id="hdMDBank" name="hdMDBank" type="hidden"  value="' + bankname + '" />';
        thtm += '<input id="hdMDaccountno" name="hdMDaccountno" type="hidden"  value="' + accountno + '" />';
        thtm += '<input id="hdMDutrno" name="hdMDutrno" type="hidden"  value="' + UTRnos + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
    }
    if ($("#idst option:selected").index() === 3) {

        thtmltable += '<tr>';
        thtmltable += '<th>Wallet Name</th>';
        thtmltable += '<th>' + walletname.text() + '</th>';
        thtmltable += '</tr>';

        thtmltable += '<tr>';
        thtmltable += '<th>Wallet-No </th>';
        thtmltable += '<th>' + walletno + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Transation-No </th>';
        thtmltable += '<th>' + transectionno + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Amount </th>';
        thtmltable += '<th>' + amount + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Comment </th>';
        thtmltable += '<th>' + comment + '</th>';
        thtmltable += '</tr>';


        thtm += '<input id="hdMDwallet" name="hdMDwallet" type="hidden"  value="' + walletname.text() + '" />';
        thtm += '<input id="hdMDwalletno" name="hdMDwalletno" type="hidden"  value="' + walletno + '" />';
        thtm += '<input id="hdMDtransationno" name="hdMDtransationno" type="hidden"  value="' + transectionno + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#idst option:selected").index() === 4) {


        thtmltable += '<tr>';
        thtmltable += '<th>Amount </th>';
        thtmltable += '<th>' + amount + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Comment </th>';
        thtmltable += '<th>' + comment + '</th>';
        thtmltable += '</tr>';


        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    if ($("#idst option:selected").index() === 5) {

        thtmltable += '<tr>';
        thtmltable += '<th>Settelment </th>';
        thtmltable += '<th>' + setteltype + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Amount </th>';
        thtmltable += '<th>' + amount + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Comment </th>';
        thtmltable += '<th>' + comment + '</th>';
        thtmltable += '</tr>';


        thtm += '<input id="hdMDsettelment" name="hdMDsettelment" type="hidden"  value="' + setteltype + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';


    }
    if ($("#idst option:selected").index() === 6) {


        thtmltable += '<tr>';
        thtmltable += '<th>CreditDetail </th>';
        thtmltable += '<th>' + crdetails + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Amount </th>';
        thtmltable += '<th>' + amount + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Comment </th>';
        thtmltable += '<th>' + comment + '</th>';
        thtmltable += '</tr>';



        thtm += '<input id="hdMDCreditDetail" name="hdMDCreditDetail" type="hidden"  value="' + crdetails + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';

    }
    if ($("#idst option:selected").index() === 7 || $("#idst option:selected").index() === 8) {


        thtmltable += '<tr>';
        thtmltable += '<th>Subject </th>';
        thtmltable += '<th>' + sbject + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Amount </th>';
        thtmltable += '<th>' + amount + '</th>';
        thtmltable += '</tr>';
        thtmltable += '<tr>';
        thtmltable += '<th>Comment </th>';
        thtmltable += '<th>' + comment + '</th>';
        thtmltable += '</tr>';


        $('#m-transferid').text('')
        thtm += '<input id="hdMDsubject" name="hdMDsubject" type="hidden"  value="' + sbject + '" />';
        thtm += '<input id="hdPaymentAmount" name="hdPaymentAmount" type="hidden"  value="' + amount + '" />';
        thtm += '<input id="hdMDComments" name="hdMDComments" type="hidden"  value="' + comment + '" />';
    }
    thtm += '</div>';

    $.ajax({
        url: callurl,
        success: function (data) {
            $('#lbltransferid').html('<b>' + data + '</b>')

        }

    })


    $('#frmdata').hide();

    thtmltable += '</table>';
    $('#tblshowinform').html(thtmltable);
    $('.txtcodecodec').val('');
    $('#showinformation').show();
    $('#CallMASTER').show();

    $('#hdninputvalues').html(thtm);
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
