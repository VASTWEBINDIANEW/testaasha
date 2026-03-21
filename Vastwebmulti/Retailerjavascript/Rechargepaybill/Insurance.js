/* get Insurance dropdown text */

function getinsurancedropdowntext(x) {
    if (x == "ICICI Prudential Life Insurance") {
        document.getElementById("txtpolicyno").placeholder = "Enter Policy Number";
        $("#txtpolicyno").val("");
        $("#insuranceamount").val("");
        $("#txtDOB").val("");
        document.getElementById("insurancemessage").innerHTML = "";
        document.getElementById("dobinsurancemessage").innerHTML = "";
        document.getElementById("insuranceamountmessage").innerHTML = "";
        document.getElementById("divbob").style.display = "block";
     }
    if (x == "Tata AIA Life Insurance") {
        document.getElementById("txtpolicyno").placeholder = "Enter Policy Number";
        $("#txtpolicyno").val("");
        $("#insuranceamount").val("");
        $("#txtDOB").val("");
        document.getElementById("insurancemessage").innerHTML = "";
        document.getElementById("dobinsurancemessage").innerHTML = "";
        document.getElementById("insuranceamountmessage").innerHTML = "";
        document.getElementById("divbob").style.display = "block";

    }
    if (x == "Tata AIG General Insurance") {
        document.getElementById("txtpolicyno").placeholder = "Enter Policy Number";
        $("#txtpolicyno").val("");
        $("#insuranceamount").val("");
        $("#txtDOB").val("");
        document.getElementById("insurancemessage").innerHTML = "";
        document.getElementById("dobinsurancemessage").innerHTML = "";
        document.getElementById("insuranceamountmessage").innerHTML = "";
        document.getElementById("divbob").style.display = "block";

    }
    if (x == "Bharti Axa Life Insurance") {
        document.getElementById("txtpolicyno").placeholder = "Enter Policy Number";
        $("#txtpolicyno").val("");
        $("#insuranceamount").val("");
        $("#txtDOB").val("");
        document.getElementById("insurancemessage").innerHTML = "";
        document.getElementById("dobinsurancemessage").innerHTML = "";
        document.getElementById("insuranceamountmessage").innerHTML = "";
        document.getElementById("divbob").style.display = "block";

    }
    if (x == "INDIA FIRST Life Insurance") {
        document.getElementById("txtpolicyno").placeholder = "Enter Policy Number";
        $("#txtpolicyno").val("");
        $("#insuranceamount").val("");
        $("#txtDOB").val("");
        document.getElementById("insurancemessage").innerHTML = "";
        document.getElementById("dobinsurancemessage").innerHTML = "";
        document.getElementById("insuranceamountmessage").innerHTML = "";
        document.getElementById("divbob").style.display = "none";

    }

    //if (x == "Life Insurance Corporation") {
    //    document.getElementById("txtpolicyno").placeholder = "Enter Policy Number";
    //    $("#txtpolicyno").val("");
    //    $("#insuranceamount").val("");
    //    $("#txtDOB").val("");
    //    $("#txtDOB").hide();
    //    document.getElementById("insurancemessage").innerHTML = "";
    //    document.getElementById("dobinsurancemessage").innerHTML = "";
    //    document.getElementById("insuranceamountmessage").innerHTML = "";
    //    document.getElementById("divbob").style.display = "none";

    //}
    //if (x.toUpperCase().search('LIFE INSURANCE CORPORATION') != -1) {
    //    document.getElementById("txtpolicyno").placeholder = "Enter Policy Number";
    //    $("#txtpolicyno").val("");
    //    $("#insuranceamount").val("");
    //    $("#txtDOB").val("");
    //    $("#txtDOB").hide();
    //    document.getElementById("insurancemessage").innerHTML = "";
    //    document.getElementById("dobinsurancemessage").innerHTML = "";
    //    document.getElementById("insuranceamountmessage").innerHTML = "";
    //    document.getElementById("divbob").style.display = "none";

    //}
   
}


/*Insurance  ICICI Prudential Insurance continue button function*/

function getinsurancecontiunebtn(insuranceddl, txtpolicyno, dob, amount) {
    /*ICICI Prudential Life Insurance*/

    if ((txtpolicyno == "" || amount == "" || dob == "") && insuranceddl == "ICICI Prudential Life Insurance") {
        if (txtpolicyno == "") {
            document.getElementById("insurancemessage").innerHTML = "Please enter policy number";
        }
        else {
            document.getElementById("insurancemessage").innerHTML = "";
        }
        if (dob == "") {
            document.getElementById("dobinsurancemessage").innerHTML = "Required Date of Birth";
        }
        else {
            document.getElementById("dobinsurancemessage").innerHTML = "";
        }
        if (amount == "") {
            document.getElementById("insuranceamountmessage").innerHTML = "Please enter amount";
        }
        else {
            document.getElementById("insuranceamountmessage").innerHTML = "";
        }
        event.preventDefault();
    }
    else {
        if (insuranceddl == "ICICI Prudential Life Insurance") {
            document.getElementById("insurancemessage").innerHTML = "";
            document.getElementById("dobinsurancemessage").innerHTML = "";
            document.getElementById("insuranceamountmessage").innerHTML = "";
            insurno = document.getElementById("txtpolicyno").value.length;
            if (insurno != 8 && insurno != "" && insurno != null) {
                document.getElementById("insurancemessage").innerHTML = "Number should be [8] digits";
                document.getElementById("insurancemessage").style.display = "block";
                event.preventDefault();
            }
            else {
                document.getElementById("insurancemessage").innerHTML = "";
                document.getElementById("dobinsurancemessage").innerHTML = "";
                document.getElementById("insuranceamountmessage").innerHTML = "";
                return true;
                //return true;
            }
        }
    }

    /*Tata AIA Life Insurance*/

    if ((txtpolicyno == "" || amount == "" || dob == "") && insuranceddl == "Tata AIA Life Insurance") {
          if (txtpolicyno == "") {
              document.getElementById("insurancemessage").innerHTML = "Please enter policy number";
          }
          else {
              document.getElementById("insurancemessage").innerHTML = "";
          }
          if (dob == "") {
              document.getElementById("dobinsurancemessage").innerHTML = "Required Date of Birth";
          }
          else {
              document.getElementById("dobinsurancemessage").innerHTML = "";
          }
          if (amount == "") {
              document.getElementById("insuranceamountmessage").innerHTML = "Please enter amount";
          }
          else {
              document.getElementById("insuranceamountmessage").innerHTML = "";
          }
          event.preventDefault();
      }
      else {
        if (insuranceddl == "Tata AIA Life Insurance") {
              document.getElementById("insurancemessage").innerHTML = "";
              document.getElementById("dobinsurancemessage").innerHTML = "";
              document.getElementById("insuranceamountmessage").innerHTML = "";
              insurno = document.getElementById("txtpolicyno").value.length;
              if (insurno != 8 && insurno != "" && insurno != null) {
                  document.getElementById("insurancemessage").innerHTML = "Number should be [8] digits";
                  document.getElementById("insurancemessage").style.display = "block";
                  event.preventDefault();
              }
              else {
                  document.getElementById("insurancemessage").innerHTML = "";
                  document.getElementById("dobinsurancemessage").innerHTML = "";
                  document.getElementById("insuranceamountmessage").innerHTML = "";
                  return true;
                  //return true;
              }
          }
      }

    /*Tata AIG General Insurance*/

    if ((txtpolicyno == "" || amount == "" || dob == "") && insuranceddl == "Tata AIG General Insurance") {
          if (txtpolicyno == "") {
              document.getElementById("insurancemessage").innerHTML = "Please enter policy number";
          }
          else {
              document.getElementById("insurancemessage").innerHTML = "";
          }
          if (dob == "") {
              document.getElementById("dobinsurancemessage").innerHTML = "Required Date of Birth";
          }
          else {
              document.getElementById("dobinsurancemessage").innerHTML = "";
          }
          if (amount == "") {
              document.getElementById("insuranceamountmessage").innerHTML = "Please enter amount";
          }
          else {
              document.getElementById("insuranceamountmessage").innerHTML = "";
          }
          event.preventDefault();
      }
      else {
        if (insuranceddl == "Tata AIG General Insurance") {
              document.getElementById("insurancemessage").innerHTML = "";
              document.getElementById("dobinsurancemessage").innerHTML = "";
              document.getElementById("insuranceamountmessage").innerHTML = "";
              insurno = document.getElementById("txtpolicyno").value.length;
              if (insurno != 10 && insurno != "" && insurno != null) {
                  document.getElementById("insurancemessage").innerHTML = "Number should be [10] digits";
                  document.getElementById("insurancemessage").style.display = "block";
                  event.preventDefault();
              }
              else {
                  document.getElementById("insurancemessage").innerHTML = "";
                  document.getElementById("dobinsurancemessage").innerHTML = "";
                  document.getElementById("insuranceamountmessage").innerHTML = "";
                  return true;
                  //return true;
              }
          }
      }

    /*Tata AIG General Insurance*/

    if ((txtpolicyno == "" || amount == "" || dob == "") && insuranceddl == "Bharti Axa Life Insurance") {
          if (txtpolicyno == "") {
              document.getElementById("insurancemessage").innerHTML = "Please enter policy number";
          }
          else {
              document.getElementById("insurancemessage").innerHTML = "";
          }
          if (dob == "") {
              document.getElementById("dobinsurancemessage").innerHTML = "Required Date of Birth";
          }
          else {
              document.getElementById("dobinsurancemessage").innerHTML = "";
          }
          if (amount == "") {
              document.getElementById("insuranceamountmessage").innerHTML = "Please enter amount";
          }
          else {
              document.getElementById("insuranceamountmessage").innerHTML = "";
          }
          event.preventDefault();
      }
      else {
        if (insuranceddl == "Bharti Axa Life Insurance") {
              document.getElementById("insurancemessage").innerHTML = "";
              document.getElementById("dobinsurancemessage").innerHTML = "";
              document.getElementById("insuranceamountmessage").innerHTML = "";
              insurno = document.getElementById("txtpolicyno").value.length;
              if (insurno != 11 && insurno != "" && insurno != null) {
                  document.getElementById("insurancemessage").innerHTML = "Number should be [11] digits";
                  document.getElementById("insurancemessage").style.display = "block";
                  event.preventDefault();
              }
              else {
                  document.getElementById("insurancemessage").innerHTML = "";
                  document.getElementById("dobinsurancemessage").innerHTML = "";
                  document.getElementById("insuranceamountmessage").innerHTML = "";
                  return true;
                  //return true;
              }
          }
      }
 
    /*Tata AIG General Insurance*/

    if ((txtpolicyno == "" || amount == "" || dob == "") && insuranceddl == "INDIA FIRST Life Insurance") {
          if (txtpolicyno == "") {
              document.getElementById("insurancemessage").innerHTML = "Please enter policy number";
          }
          else {
              document.getElementById("insurancemessage").innerHTML = "";
          }
          if (dob == "") {
              document.getElementById("dobinsurancemessage").innerHTML = "Required Date of Birth";
          }
          else {
              document.getElementById("dobinsurancemessage").innerHTML = "";
          }
          if (amount == "") {
              document.getElementById("insuranceamountmessage").innerHTML = "Please enter amount";
          }
          else {
              document.getElementById("insuranceamountmessage").innerHTML = "";
          }
          event.preventDefault();
      }
      else {
        if (insuranceddl == "INDIA FIRST Life Insurance") {
              document.getElementById("insurancemessage").innerHTML = "";
              document.getElementById("dobinsurancemessage").innerHTML = "";
              document.getElementById("insuranceamountmessage").innerHTML = "";
              insurno = document.getElementById("txtpolicyno").value.length;
              if ((insurno < 8 || insurno > 16) && insurno != "" && insurno != null) {
                  document.getElementById("insurancemessage").innerHTML = "Number should be [8-16] digits";
                  document.getElementById("insurancemessage").style.display = "block";
                  event.preventDefault();
              }
              else {
                  document.getElementById("insurancemessage").innerHTML = "";
                  document.getElementById("dobinsurancemessage").innerHTML = "";
                  document.getElementById("insuranceamountmessage").innerHTML = "";
                  return true;
                  //return true;
              }
          }
    }


/* Life Insurance Corporation */

    if ((txtpolicyno == "" || amount == "") && insuranceddl.toUpperCase().search('LIFE INSURANCE CORPORATION') != -1) {
        if (txtpolicyno == "") {
            document.getElementById("insurancemessage").innerHTML = "Please enter policy number";
        }
        else {
            document.getElementById("insurancemessage").innerHTML = "";
        }
        //if (dob == "") {
        //    document.getElementById("dobinsurancemessage").innerHTML = "Required Date of Birth";
        //}
        //else {
        //    document.getElementById("dobinsurancemessage").innerHTML = "";
        //}
        if (amount == "") {
            document.getElementById("insuranceamountmessage").innerHTML = "Please enter amount";
        }
        else {
            document.getElementById("insuranceamountmessage").innerHTML = "";
        }
        event.preventDefault();
    }
    else {
        if (insuranceddl.toUpperCase().search('LIFE INSURANCE CORPORATION') != -1) {
            document.getElementById("insurancemessage").innerHTML = "";
            document.getElementById("dobinsurancemessage").innerHTML = "";
            document.getElementById("insuranceamountmessage").innerHTML = "";
            insurno = document.getElementById("txtpolicyno").value.length;
            if ((insurno < 8 || insurno > 16) && insurno != "" && insurno != null) {
                document.getElementById("insurancemessage").innerHTML = "Number should be [8-16] digits";
                document.getElementById("insurancemessage").style.display = "block";
                event.preventDefault();
            }
            else {
                document.getElementById("insurancemessage").innerHTML = "";
                document.getElementById("dobinsurancemessage").innerHTML = "";
                document.getElementById("insuranceamountmessage").innerHTML = "";
                return true;
                //return true;
            }
        }
    }


    return true;
}
