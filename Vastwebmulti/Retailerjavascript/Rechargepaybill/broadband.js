/* select broadband dropdown text change placeholder broadband  text box */
/* create function*/
function getbroadbandddltext(x) {
    if (x == "Connect Broadband") {
        document.getElementById("txtbroadbandno").placeholder = "Enter Customer ID Number";
        document.getElementById("txtbroadbandno").value = "";
        document.getElementById("broadbandmessage").innerHTML = "";
        document.getElementById("broadbandamountmessage").innerHTML = "";
    }

    if (x == "Hathway Broadband") {
        document.getElementById("txtbroadbandno").placeholder = "Enter Customer ID";
        document.getElementById("txtbroadbandno").value = "";
        document.getElementById("broadbandmessage").innerHTML = "";
        document.getElementById("broadbandamountmessage").innerHTML = "";
    }

    if (x == "Tikona Broadband") {
        document.getElementById("txtbroadbandno").placeholder = "Enter Service ID";
        document.getElementById("txtbroadbandno").value = "";
        document.getElementById("broadbandmessage").innerHTML = "";
        document.getElementById("broadbandamountmessage").innerHTML = "";
    }
    if (x == "Nextra Broadband") {
        document.getElementById("txtbroadbandno").placeholder = "Enter Customer ID";
        document.getElementById("txtbroadbandno").value = "";
        document.getElementById("broadbandmessage").innerHTML = "";
        document.getElementById("broadbandamountmessage").innerHTML = "";
    }
    if (x == "Spectranet Broadband") {
        document.getElementById("txtbroadbandno").placeholder = "Enter Account ID";
        document.getElementById("txtbroadbandno").value = "";
        document.getElementById("broadbandmessage").innerHTML = "";
        document.getElementById("broadbandamountmessage").innerHTML = "";
    }
    if (x == "TTN BroadBand") {
        document.getElementById("txtbroadbandno").placeholder = "Enter Customer ID";
        document.getElementById("txtbroadbandno").value = "";
        document.getElementById("broadbandmessage").innerHTML = "";
        document.getElementById("broadbandamountmessage").innerHTML = "";
    }
  
}

/* get Connect Broadband" continue button value */

function  getbroadbandcontiunebtn(broadbandddltext, broadbandno , amount)
{
    if ((broadbandno == ""  || amount =="")&& broadbandddltext == "Connect Broadband")
    {
        if(broadbandno == "" )
        {
            document.getElementById("broadbandmessage").innerHTML = "Please enter customer-id ";
        }
        else
        {
            document.getElementById("broadbandmessage").innerHTML = "";
        }
        if (amount == "")
        {
            document.getElementById("broadbandamountmessage").innerHTML = "Please enter amount";
        }
        else {
            document.getElementById("broadbandamountmessage").innerHTML = "";
        }
        event.preventDefault();
    }
    else
    {
        if(broadbandddltext != "Connect Broadband")
        {
             document.getElementById("broadbandmessage").innerHTML = "";
            var brobandval = document.getElementById("txtbroadbandno").value.length;
            if ((brobandval < 4 || brobandval > 11) && brobandval !== null && brobandval !== "") 
            {
           
                document.getElementById("broadbandmessage").innerHTML = "Number should be [4-11] digits";
                document.getElementById("broadbandmessage").style.display = "block";
                event.preventDefault();
            }
            else
            {
                document.getElementById("broadbandmessage").innerHTML = "";
                document.getElementById("broadbandamountmessage").innerHTML = "";
                return true;
                //return true;
            }
        }
    }

    /*Hathway Broadband*/
    if ((broadbandno == "" || amount == "") && broadbandddltext == "Hathway Broadband") {
        if (broadbandno == "") {
            document.getElementById("broadbandmessage").innerHTML = "Please enter user-Id ";
        }
        else {
            document.getElementById("broadbandmessage").innerHTML = "";
        }
        if (amount == "") {
            document.getElementById("broadbandamountmessage").innerHTML = "Please enter amount";
        }
        else {
            document.getElementById("broadbandamountmessage").innerHTML = "";
        }
        event.preventDefault();
    }
    else {
        if (broadbandddltext == "Hathway Broadband") {

            document.getElementById("broadbandmessage").innerHTML = "";
            var brobandval = document.getElementById("txtbroadbandno").value.length;
            if ((brobandval < 9 || brobandval > 15) && brobandval !== null && brobandval !== "") {

                document.getElementById("broadbandmessage").innerHTML = "Number should be [9-15] digits";
                document.getElementById("broadbandmessage").style.display = "block";
                event.preventDefault();
            }
            else {
                document.getElementById("broadbandmessage").innerHTML = "";
                document.getElementById("broadbandamountmessage").innerHTML = "";
                return true;
                //return true;
            }
        }
    }

    /*Tikona Broadband*/

    if ((broadbandno == "" || amount == "") && broadbandddltext == "Tikona Broadband") {
        if (broadbandno == "") {
            document.getElementById("broadbandmessage").innerHTML = "Please enter service-Id ";
        }
        else {
            document.getElementById("broadbandmessage").innerHTML = "";
        }
        if (amount == "") {
            document.getElementById("broadbandamountmessage").innerHTML = "Please enter amount";
        }
        else {
            document.getElementById("broadbandamountmessage").innerHTML = "";
        }
        event.preventDefault();
    }
    else {
        if (broadbandddltext == "Tikona Broadband") {

            document.getElementById("broadbandmessage").innerHTML = "";
            var brobandval = document.getElementById("txtbroadbandno").value.length;
            if (brobandval !=10 && brobandval !== null && brobandval !== "") {

                document.getElementById("broadbandmessage").innerHTML = "Number should be [10] digits";
                document.getElementById("broadbandmessage").style.display = "block";
                event.preventDefault();
            }
            else {
                document.getElementById("broadbandmessage").innerHTML = "";
                document.getElementById("broadbandamountmessage").innerHTML = "";
                return true;
                //return true;
            }
        }
    }

    /*Nextra Broadband*/

    if ((broadbandno == "" || amount == "") && broadbandddltext == "Nextra Broadband") {
        if (broadbandno == "") {
            document.getElementById("broadbandmessage").innerHTML = "Please enter customer-Id ";
        }
        else {
            document.getElementById("broadbandmessage").innerHTML = "";
        }
        if (amount == "") {
            document.getElementById("broadbandamountmessage").innerHTML = "Please enter amount";
        }
        else {
            document.getElementById("broadbandamountmessage").innerHTML = "";
        }
        event.preventDefault();
    }
    else {
        if (broadbandddltext == "Nextra Broadband") {

            document.getElementById("broadbandmessage").innerHTML = "";
            var brobandval = document.getElementById("txtbroadbandno").value.length;
            if (brobandval != 7 && brobandval !== null && brobandval !== "") {

                document.getElementById("broadbandmessage").innerHTML = "Number should be [7] digits";
                document.getElementById("broadbandmessage").style.display = "block";
                event.preventDefault();
            }
            else {
                document.getElementById("broadbandmessage").innerHTML = "";
                document.getElementById("broadbandamountmessage").innerHTML = "";
                return true;
                //return true;
            }
        }
    }

    /*Spectranet Broadband*/

    if ((broadbandno == "" || amount == "") && broadbandddltext == "Spectranet Broadband") {
        if (broadbandno == "") {
            document.getElementById("broadbandmessage").innerHTML = "Please enter account-Id ";
        }
        else {
            document.getElementById("broadbandmessage").innerHTML = "";
        }
        if (amount == "") {
            document.getElementById("broadbandamountmessage").innerHTML = "Please enter amount";
        }
        else {
            document.getElementById("broadbandamountmessage").innerHTML = "";
        }
        event.preventDefault();
    }
    else {
        if (broadbandddltext == "Spectranet Broadband") {

            document.getElementById("broadbandmessage").innerHTML = "";
            var brobandval = document.getElementById("txtbroadbandno").value.length;
            if ((brobandval < 6 || brobandval > 30) && brobandval !== null && brobandval !== "") {

                document.getElementById("broadbandmessage").innerHTML = "Number should be [6-30] digits";
                document.getElementById("broadbandmessage").style.display = "block";
                event.preventDefault();
            }
            else {
                document.getElementById("broadbandmessage").innerHTML = "";
                document.getElementById("broadbandamountmessage").innerHTML = "";
                return true;
                //return true;
            }
        }
    }

    /*TTN BroadBand*/

    if ((broadbandno == "" || amount == "") && broadbandddltext == "TTN BroadBand") {
        if (broadbandno == "") {
            document.getElementById("broadbandmessage").innerHTML = "Please enter customer-Id ";
        }
        else {
            document.getElementById("broadbandmessage").innerHTML = "";
        }
        if (amount == "") {
            document.getElementById("broadbandamountmessage").innerHTML = "Please enter amount";
        }
        else {
            document.getElementById("broadbandamountmessage").innerHTML = "";
        }
        event.preventDefault();
    }
    else {
        if (broadbandddltext == "TTN BroadBand") {

            document.getElementById("broadbandmessage").innerHTML = "";
            var brobandval = document.getElementById("txtbroadbandno").value.length;
            if ((brobandval < 6 || brobandval > 30) && brobandval !== null && brobandval !== "") {

                document.getElementById("broadbandmessage").innerHTML = "Number should be [6-30] digits";
                document.getElementById("broadbandmessage").style.display = "block";
                event.preventDefault();
            }
            else {
                document.getElementById("broadbandmessage").innerHTML = "";
                document.getElementById("broadbandamountmessage").innerHTML = "";
                return true;
                //return true;
            }
        }
    }
}