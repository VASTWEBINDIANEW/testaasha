// Note:-  Every Hotel room must contains  1 - 8 Adults and/Or 0 - 2 Child
// Child age must be 0-12 years old only.

//////// Multi City Script////
var jq = jQuery.noConflict(true);
function GetDynamicTextbox(value) {
    return '<div class="row"><div class="col-md-4"><div class="row" style="margin-bottom:-29px;"><div class="col-md-6"><div class="form-group" style="margin-bottom:0px;"><div class="input-group" style="margin-bottom:0px; margin-top:-2px;border:1px solid #dddddd; height:37px;margin-left:1px;"><input type="text" name="txtSource" class="form-control hoja" placeholder="From" style="padding-left:10px;font-size:16px;" autocomplete="off"  /><div class="input-group-addon"><img src="/A2ZMultiService/images/departure.png" class="img-respinsive" style="height:21px; margin-right:-7px;" /></div></div></div></div><div class="col-md-6"><div class="form-group" style="margin-bottom:0px;"><div class="input-group" style="margin-bottom:0px; margin-top:-2px;border:1px solid #dddddd; height:37px;margin-left:1px;"><input type="text" class="form-control hoja"  name="txtDestination" placeholder="TO" style="padding-left:10px;font-size:16px;" autocomplete="off" /><div class="input-group-addon"><img src="/A2ZMultiService/images/arrival.png" class="img-respinsive" style="height:21px;margin-right:-7px;" /></div></div></div></div></div></div><div class="col-md-2"><div class="form-group" style="margin-bottom:0px;"><div class="input-group" style="margin-bottom:0px; margin-top:-2px;border:1px solid #dddddd; height:37px;margin-left:1px;"><input type="text" id="txt_frm_date" name="txt_frm_date" class="datepicker form-control" placeholder="Depart Date" style="padding-left:10px;font-size:16px;" /><div class="input-group-addon"><img src="/A2ZMultiService/images/departurescal.png" class="img-respinsive" style="height:21px;margin-right:-7px;" /></div></div></div></div><div class="col-md-1 p-r-0 p-l-0" onclick="RemoveTextBox(this)" style="width: 2%;margin-top: 7px;"><span><i class="fa fa-times"></i></span></div><div class="col-md-2"><div class="form-group" style="margin-bottom:0px;"><button type="button" onclick="AddTextBox()" class="btn bg-blue-grey waves-effect" style="height:37px; float:left;">Add City</button></div></div></div>';
}
function AddTextBox() {
    jq("#divForSpacialReturn").hide();
    var div = document.createElement('DIV');
    div.innerHTML = GetDynamicTextbox("");
    document.getElementById("multicityDynamic").appendChild(div);    jq(".hoja").autocomplete({
        //source: '/A2ZMultiService/RETAILER/Home/GetSourceName', //For localhost        source: '/RETAILER/Home/GetSourceName', //For Live        minLength: 3
    })}
function RemoveTextBox(div) {

    document.getElementById("multicityDynamic").removeChild(div.parentNode.parentNode);

}
function ReturnTypeSet()
{
    var isLCC = jq('input[name="ReturnType"]:checked').val();
    //alert(isLCC);
    if(isLCC != null && isLCC != '')
    {
        if(isLCC == "LCC")
        {
            jq('#md_checkbox_24').click();
            jq('#md_checkbox_24').attr('checked', false);
            jq('#md_checkbox_25').click();
            jq('#md_checkbox_25').attr('checked', false);
            jq('#md_checkbox_26').click();
            jq('#md_checkbox_26').attr('checked', false);
            jq("#GDScheckBoxes").hide();
            jq("#LCCcheckBoxes").show();
            jq("#md_checkbox_21").click();
            jq("#md_checkbox_22").click();
            jq("#md_checkbox_23").click();
        }
        
        if (isLCC == "GDS") {
            
            jq('#md_checkbox_21').click();
            jq('#md_checkbox_21').attr('checked', false);
            jq('#md_checkbox_22').click();
            jq('#md_checkbox_22').attr('checked', false);
            jq('#md_checkbox_23').click();
            jq('#md_checkbox_23').attr('checked', false);
            //jq("input:checkbox").attr('checked', false);
            jq("#LCCcheckBoxes").hide();
            jq("#GDScheckBoxes").show();
            //jq("input:checkbox.GDS").attr('checked', 'checked');
            jq("#md_checkbox_24").click();
            jq("#md_checkbox_25").click();
            jq("#md_checkbox_26").click();
            
            
            
        }
        
    }

}
function AdvanceSearchSet()
{
    //alert("AdvanceSearchSet()");
    var isOneWay = jq('input[name="AdvanceSearchType"]:checked').val();
   // alert(isOneWay);
    if(isOneWay != null && isOneWay != '')
    {
        if(isOneWay == "Oneway")
        {
            document.getElementById("txt_to_date").disabled = true;
            document.getElementById("txt_to_date").style.color = "white";
            document.getElementById('txt_to_date').style.backgroundColor = "white";
            document.getElementById("txt_to_date").style.paddingLeft = "10px";
            document.getElementById("txt_to_date").style.fontSize = "16px";
        }
        if (isOneWay == "Return") {
            document.getElementById("txt_to_date").disabled = false;
            document.getElementById("txt_to_date").style = backgroundColor = "white";
            document.getElementById("txt_to_date").style.paddingLeft = "10px";
            document.getElementById("txt_to_date").style.fontSize = "16px";
        }
    }
    $("#multicityDynamic").empty();
}
//////////// End /////////////


//////////////////// Hotel ////////////////////
var roomGuestMapObjects = {};
var RoomNodes = [];
var ChildAges = [];
var roomItem = { "NoOfAdults": 1, "NoOfChild": 0, "ChildAge": ChildAges };
RoomNodes.push(roomItem);
roomGuestMapObjects.RoomNodes = RoomNodes

/* Hotel Room Guest Configuration */
/* ------ ADULT Config ----------*/
function btnHotelAdultMinus (dom) {
    //alert();
    var roomIndex = parseInt(jq(dom).parent().attr("data-room"));
    //alert("room Index" + roomIndex);
    if (roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfAdults > 1) {
        roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfAdults--;
        //console.log(JSON.stringify(roomGuestMapObjects));
        jq(dom).prev().find("span").html(roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfAdults);
    } else {
        alert("Atleast one adult is required!");
    }
    jq("#roomsDetails").val(JSON.stringify(roomGuestMapObjects));
    console.log(JSON.stringify(roomGuestMapObjects));
}; 
function btnHotelAdlutPlus(dom) {
    //alert();
    var roomIndex = parseInt(jq(dom).parent().attr("data-room"));
    //alert("room Index" + roomIndex);
    if (roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfAdults < 5) {
        roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfAdults++;
        //console.log(JSON.stringify(roomGuestMapObjects));
        jq(dom).prev().prev().find("span").html(roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfAdults);
    } else {
        alert("Only five adults is allowed per room!");
    }
    jq("#roomsDetails").val(JSON.stringify(roomGuestMapObjects));
    console.log(JSON.stringify(roomGuestMapObjects));
};
/* ------ ADULT Config  END----------*/
/* ------ Child Config ----------*/
function btnHotelChildMinus(dom) {
    //alert();
    var roomIndex = parseInt(jq(dom).parent().attr("data-room"));
    //alert("room Index" + roomIndex);
    if (roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfChild > 0) {
        roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfChild--;
        jq(dom).prev().find("span").html(roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfChild);
        var childAgeLi = parseInt(roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfChild );
        //alert(childAgeLi);
        //jq("li[data-child-age='" + childAgeLi + "']").hide();
        if (childAgeLi == 0) {
            jq(dom).next().next().hide();
        } else {
            jq(dom).next().next().next().hide();
        }
        roomGuestMapObjects.RoomNodes[roomIndex - 1].ChildAge.pop();
        jq("#roomsDetails").val(JSON.stringify(roomGuestMapObjects));
        console.log(JSON.stringify(roomGuestMapObjects));
    } 
};
function btnHotelChildPlus(dom) {
    //alert();
    var roomIndex = parseInt(jq(dom).parent().attr("data-room"));
   // alert("room Index" + roomIndex);
    if (roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfChild < 2) {
        roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfChild++;
        jq(dom).prev().prev().find("span").html(roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfChild);
        var childAgeLi = parseInt(roomGuestMapObjects.RoomNodes[roomIndex - 1].NoOfChild - 1);
          //alert(childAgeLi);
        //jq("li[data-child-age='" + childAgeLi + "']").show();
          if (childAgeLi == 0)
          {
              jq(dom).next().show();
          } else {
              jq(dom).next().next().show();
          }
          roomGuestMapObjects.RoomNodes[roomIndex - 1].ChildAge.push(0);
          jq("#roomsDetails").val(JSON.stringify(roomGuestMapObjects));
        console.log(JSON.stringify(roomGuestMapObjects));
    }
    else {
        alert("Only two childs is allowed per room!");
    }

};
//jq(".ddlChildAge").change(function () {
//    var end = parseInt(this.value);
//    //alert(end);
//    var childAgeArrayIndex = parseInt(jq(this).parent().attr("data-child-age"));
//    //alert(childAgeArrayIndex);
//    var roomIndex = parseInt(jq(this).parent().parent().attr("data-room"));
//    roomGuestMapObjects.RoomNodes[roomIndex - 1].ChildAge[childAgeArrayIndex] = end;
//    jq("#roomsDetails").val(JSON.stringify(roomGuestMapObjects));
//    console.log(JSON.stringify(roomGuestMapObjects));
//});
function ddlChildAge (dom) {
    var end = parseInt(dom.value);
    //alert(end);
    var childAgeArrayIndex = parseInt(jq(dom).parent().attr("data-child-age"));
    //alert(childAgeArrayIndex);
    var roomIndex = parseInt(jq(dom).parent().parent().attr("data-room"));
    roomGuestMapObjects.RoomNodes[roomIndex - 1].ChildAge[childAgeArrayIndex] = end;
    jq("#roomsDetails").val(JSON.stringify(roomGuestMapObjects));
    console.log(JSON.stringify(roomGuestMapObjects));
};
/* ------ Child Config END----------*/

/*  ---------------- ADD OR REMOVE dynamic Room -----------------------*/
function GetDynamicHotelTextbox(value) {
     //return'<ul style="display:flex;list-style:none;padding:7px;" ><!--  Adult--><li style="padding-top:7px;"><span class="showHotelAdultCount" style="padding-right:4px;padding-left:4px;">1</span>Adult</li><li class="btnHotelAdultMinus"><button type="button" class="btn  waves-effect btmPlusMinus" style="margin-right: 0px;margin-left: 20px; background:white; border:1px solid #cdcecf;box-shadow:none;"><i class="fa fa-minus" aria-hidden="true" style="font-size:10px;color:#757979;"></i></button></li><li class="btnHotelAdlutPlus"><button type="button" class="btn  waves-effect btmPlusMinus" style="background:white; border:1px solid #cdcecf;box-shadow:none; border-left:none;"><i class="fa fa-plus" aria-hidden="true" style="font-size:10px;color:#757979;"></i></button></li><!--  Child--><li style="padding-top:7px;"><span id="showAdultCount" style="padding-right:4px;padding-left:4px;">0</span>Child</li><li class="btnHotelChildMinus"><button type="button" class="btn  waves-effect btmPlusMinus" style="margin-right: 0px;margin-left: 20px; background:white; border:1px solid #cdcecf;box-shadow:none;"><i class="fa fa-minus" aria-hidden="true" style="font-size:10px;color:#757979;"></i></button></li><li class="btnHotelChildPlus"><button type="button" class="btn  waves-effect btmPlusMinus" style="background:white; border:1px solid #cdcecf;box-shadow:none; border-left:none;"><i class="fa fa-plus" aria-hidden="true" style="font-size:10px;color:#757979;"></i></button></li><li data-child-age="0" style="display:none;"><lable>Child 1 Age</lable><select class="ddlChildAge" style="padding:6px;"><option value="0">0</option><option value="1">1</option><option value="2">2</option><option value="3">3</option><option value="4">4</option><option value="5">5</option><option value="6">6</option><option value="7">7</option><option value="8">8</option><option value="9">9</option><option value="10">10</option><option value="11">11</option><option value="12">12</option></select></li><li data-child-age="1" style="margin-left:10px;display:none;"><lable>Child 2 Age</lable><select class="ddlChildAge" style="padding:6px;"><option value="0">0</option><option value="1">1</option><option value="2">2</option><option value="3">3</option><option value="4">4</option><option value="5">5</option><option value="6">6</option><option value="7">7</option><option value="8">8</option><option value="9">9</option><option value="10">10</option><option value="11">11</option><option value="12">12</option></select></li><li><div class="col-md-1 p-r-0 p-l-0" onclick="RemoveHotelTextBox(this)" style="width: 2%;margin-top: 10px;"><span><i class="fa fa-times"></i></span></div></li><li><div class="form-group" style="margin-bottom:0px;"><button type="button" onclick="AddHotelTextBox()" class="btn bg-blue-grey waves-effect" style="height:37px; float:left;margin-left: 26px;">Add Room</button></div></li></ul>';
     return jq(".roomConfigClone").html();
}
function AddHotelTextBox() {
    //alert("Add room");
    // jq("#divForSpacialReturn").hide();
    var div = document.createElement('DIV');
    div.innerHTML = GetDynamicHotelTextbox("");
    document.getElementById("roomGuestDynamic").appendChild(div);    var NoOfRoomsOldValue = document.getElementById("NoOfRooms").value;    document.getElementById("NoOfRooms").value = parseInt(NoOfRoomsOldValue) + parseInt("1");    document.getElementById("totalRoomsToShow").textContent = parseInt(NoOfRoomsOldValue) + parseInt("1");
    div.setAttribute("class", "row");
    ////// add node ///
    var newRoomItem = { "NoOfAdults": 1, "NoOfChild": 0, "ChildAge": [] };
    roomGuestMapObjects.RoomNodes.push(newRoomItem);
    ////////////////////////
    var newRoomIndex = roomGuestMapObjects.RoomNodes.length;
    //alert(newRoomIndex);
    //alert(div.childNodes[0].html());
    div.getElementsByTagName("UL")[0].setAttribute("data-room", newRoomIndex);
    jq("#roomsDetails").val(JSON.stringify(roomGuestMapObjects));
    console.log(JSON.stringify(roomGuestMapObjects));
}
function RemoveHotelTextBox(div) {
    document.getElementById("roomGuestDynamic").removeChild(div.parentNode.parentNode.parentNode);    var NoOfRoomsOldValue = document.getElementById("NoOfRooms").value;    document.getElementById("NoOfRooms").value = parseInt(NoOfRoomsOldValue) - parseInt("1");    //alert(parseInt(NoOfRoomsOldValue) - parseInt("1"));
    document.getElementById("totalRoomsToShow").textContent = parseInt(NoOfRoomsOldValue) - parseInt("1");
    var roomIndex = parseInt(jq(div).parent().parent().attr("data-room"));
    roomIndex = roomIndex - 1;
   // alert(roomIndex);
    //roomGuestMapObjects.RoomNodes.pop();
    roomGuestMapObjects.RoomNodes.splice(roomIndex,1);
    jq("#roomsDetails").val(JSON.stringify(roomGuestMapObjects));
    console.log(JSON.stringify(roomGuestMapObjects));
}
/*   --------------------------------------------------------------------*/
jq(function () {
    //alert("ready");
    console.log(roomGuestMapObjects);
    jq("#roomsDetails").val(JSON.stringify(roomGuestMapObjects));
});
/* ------ END ------------------- */


//////////////////////////////////////////////////