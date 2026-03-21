@model IEnumerable<Vastwebmulti.Models.Select_balance_Super_stokist_Result>
@{
    ViewBag.Title = "Admin_to_master_Dealer";
    Layout = "~/Areas/ADMIN/Views/Shared/_Layout.cshtml";
}

<section class="content">
    <div class="container-fluid">
        <div class="row clearfix">
            <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                <div class="card" style="margin-top:2px;">
                    @Html.Partial("_menuaccount")

                </div>
            </div>
        @using (Html.BeginForm("Admin_to_master_Dealer", "Home", FormMethod.Post))
        {
                <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                    <div class="card" style="margin-top:2px;">
                        <div class="row">
                            <div class="col-md-4">
                              
                               <select class="form-control show-tick" data-live-search="true">
                                        <option>Hot Dog, Fries and a Soda</option>
                                        <option>Burger, Shake and a Smile</option>
                                        <option>Sugar, Spice and all things nice</option>
                                    </select>
                           
                                 @*<div class="form-group">
                                    <div class="form-line" style="padding-top:3px;">
                         @Html.DropDownList("DealerId1", ViewBag.Dealername1 as SelectList, "Select a M-Dealer", new { id = "DealerId1", @class = "form-control",@required="required",@style= "padding-left:0px;" })
                                    </div>
                                </div>*@
                            </div>
                            <div class="col-md-3">
                                <div class="form-group">
                                    <div class="form-line" style="padding-top:3px;">
                                        <input type="text" id="txt_frm_date" name="txt_frm_date" class="datepicker form-control" placeholder="Select From Date" style="text-align:center;">
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="form-group">
                                    <div class="form-line" style="padding-top:3px;">
                                        <input type="text" id="txt_to_date" name="txt_to_date" class="datepicker form-control" placeholder="Select From Date" style="text-align:center;">
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-2">
                                <center>
                                    <button type="submit" class="btn bg-indigo waves-effect" style="margin-top:8px; ">
                                        <i class="material-icons">search</i>
                                    </button>
                                </center>
                            </div>
                        </div>
                    </div>
                </div>
            }
        </div>
        @*<div class="row clearfix">
            <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12" style="margin-top:-39px;">
                <div class="card">
                    <div class="">
                        <h2 style="color:black; font-size:16px;padding-top:9px; padding-left:22px;">
                            Value Difference Details
                        </h2>
                    </div>
                    <div class="body" style="margin-top:-25px;">
                        <div class="table-responsive " style="position:inherit;">
                            <table class="table table-bordered">
                                <thead class="navbar" style="color:white;position:inherit;">
                                    <tr>
                                        <th>Port No</th>
                                        <th>Sim Number</th>
                                        <th>Operator Name</th>
                                        <th>Opening Balance</th>
                                        <th>Total Recharge </th>
                                        <th>Total purchasing </th>
                                        <th>Closing Balance </th>
                                        <th>Difference </th>
                                        <th>Edit</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    @foreach (var item in Model)
                                    {
                                        <tr>
                                            <td>@Html.DisplayFor(modelItem => item.portno)</td>
                                            <td>@Html.DisplayFor(modelItem => item.simnumber)</td>
                                            <td> @Html.DisplayFor(modelItem => item.optname)</td>
                                            <td> @Html.DisplayFor(modelItem => item.openbal)</td>
                                            <td> @Html.DisplayFor(modelItem => item.total)</td>
                                            <td> @Html.DisplayFor(modelItem => item.purcharge)</td>
                                            <td> @Html.DisplayFor(modelItem => item.closebal)</td>
                                            <td> @Html.DisplayFor(modelItem => item.diff)</td>
                                            <td>
                                                <button type="button" style="background-color:transparent; border:none;" data-toggle="modal" data-target="#defaultModal" onclick="animation('@item.portno','@item.optname','@item.purcharge')">
                                                    <i class="material-icons">edit</i>
                                                </button>
                                            </td>
                                        </tr>
                                    }
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>*@
    </div>
</section>

<link href="~/AdminDesign/plugins/bootstrap-select/css/bootstrap-select.css" rel="stylesheet" />
<!-- Jquery Core Js -->
<script src="~/AdminDesign/plugins/jquery/jquery.min.js"></script>
<script src="~/AdminDesign/js/demo.js"></script>
<script src="~/AdminDesign/plugins/bootstrap/js/bootstrap.js"></script>
<script src="~/AdminDesign/plugins/jquery-validation/jquery.validate.js"></script>

<!-- Custom Js -->
<script src="~/AdminDesign/js/pages/forms/basic-form-elements.js"></script>
<script src="~/AdminDesign/plugins/jquery-slimscroll/jquery.slimscroll.js"></script>
<script src="~/AdminDesign/plugins/bootstrap-select/js/bootstrap-select.js"></script>


<script>
    window.onload = function () {

        var from = '@ViewBag.fromdate';
        var to = '@ViewBag.todate';
        if (from != "" && to != "") {
            $('#txt_frm_date').val(from);
            $('#txt_to_date').val(to);
        }
        else {
            var month = new Array("Jan", "Feb", "Mar",
    "Apr", "May", "Jun", "Jul", "Aug", "Sep",
    "Oct", "Nov", "Dec");

            var d = new Date();
            var curr_date = d.getDate();
            var curr_month = d.getMonth();
            var curr_year = d.getFullYear();
            var tt = curr_date + "-" + month[curr_month]
            + "-" + curr_year;
            document.getElementById("txt_frm_date").value = tt;
            document.getElementById("txt_to_date").value = tt;
        }
        var dateformat = '@ViewBag.format';
        if (dateformat != null && dateformat != "") {
            swal("Oops...", dateformat, "error");
        }
       }
</script>



