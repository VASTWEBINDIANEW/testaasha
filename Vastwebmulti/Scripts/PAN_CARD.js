$(function () {
   
    
    /******** HIDE ALL Onload*******/
    $("#ProofIdentity option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();
    $("#ProofIdentity option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
    $("#ProofIdentity option[value='Trust Deed']").hide();
    $("#ProofIdentity option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
    $("#ProofIdentity option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
    $("#ProofIdentity option[value='Partnership Deed']").hide();
    $("#ProofIdentity option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
    $("#ProofIdentity option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
    $("#ProofIdentity option[value='Certificate of Identity signed by a Member of Parliament']").hide();
    $("#ProofIdentity option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
    $("#ProofIdentity option[value='Driving License']").hide();
    $("#ProofIdentity option[value='Passport']").hide();
    $("#ProofIdentity option[value='Arms license']").hide();
    $("#ProofIdentity option[value='Central Government Health Scheme Card']").hide();
    $("#ProofIdentity option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
    $("#ProofIdentity option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
    $("#ProofIdentity option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
    $("#ProofIdentity option[value='Pensioner Card having photograph of the applicant']").hide();
    $("#ProofIdentity option[value='Electors photo identity card']").hide();
    $("#ProofIdentity option[value='Ration card having photograph of the applicant']").hide();
    $("#ProofIdentity option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();

    $("#ProofAddress option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();
    $("#ProofAddress option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
    $("#ProofAddress option[value='Trust Deed']").hide();
    $("#ProofAddress option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
    $("#ProofAddress option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
    $("#ProofAddress option[value='Partnership Deed']").hide();
    $("#ProofAddress option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
    $("#ProofAddress option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
    $("#ProofAddress option[value='Certificate of Identity signed by a Member of Parliament']").hide();
    $("#ProofAddress option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
    $("#ProofAddress option[value='Driving License']").hide();
    $("#ProofAddress option[value='Passport']").hide();
    $("#ProofAddress option[value='Arms license']").hide();
    $("#ProofAddress option[value='Central Government Health Scheme Card']").hide();
    $("#ProofAddress option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
    $("#ProofAddress option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
    $("#ProofAddress option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
    $("#ProofAddress option[value='Pensioner Card having photograph of the applicant']").hide();
    $("#ProofAddress option[value='Electors photo identity card']").hide();
    $("#ProofAddress option[value='Ration card having photograph of the applicant']").hide();
    $("#ProofAddress option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();

    ///
    $("#ProofDOB").prop("disabled", true);
    ///
    /**************** Category Change event for Identity proof********/
    $("#Category").change(function () {
        /**************** Show adhaar/Hide Aadhaar ********/
        if ($(this).val() == "Individual") {
            $("#nameaadhaar").show();
        } else {
            $("#nameaadhaar").hide();
        }
        /**************** DOB enable/disable ********/
        if ($(this).val() == "Individual" || $(this).val() == "Body of Individuals" || $(this).val() == "Hindu Undivided Family" || $(this).val() == "Association of Persons" || $(this).val() == "Local Authority" || $(this).val() == "Artificial Juridical Person" || $(this).val() == "Government") {
            $("#ProofDOB").prop("disabled", false);
        } else {
            $("#ProofDOB").prop("disabled", true);
        }

        /**************** Bind Proof of Identitydrop down options  (id = ProofIdentity)********/
        //alert("filter " + fileter);
        if ($(this).val() == "Individual" || $(this).val() == "Body of Individuals" || $(this).val() == "Hindu Undivided Family" || $(this).val() == "Association of Persons" || $(this).val() == "Local Authority" || $(this).val()  == "Artificial Juridical Person" ||  $(this).val() == "Government") {
            /******** SHOW *******/
            $("#ProofIdentity option[value='Certificate of Identity signed by a Gazetted Officer']").show();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Member of Legislative Assembly']").show();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Member of Parliament']").show();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Municipal Councillor']").show();
            $("#ProofIdentity option[value='Driving License']").show();
            $("#ProofIdentity option[value='Passport']").show();
            $("#ProofIdentity option[value='Arms license']").show();
            $("#ProofIdentity option[value='Central Government Health Scheme Card']").show();
            $("#ProofIdentity option[value='Ex-Servicemen Contributory Health Scheme photo card']").show();
            $("#ProofIdentity option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").show();
            $("#ProofIdentity option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").show();
            $("#ProofIdentity option[value='Pensioner Card having photograph of the applicant']").show();
            $("#ProofIdentity option[value='Electors photo identity card']").show();
            $("#ProofIdentity option[value='Ration card having photograph of the applicant']").show();
            $("#ProofIdentity option[value='AADHAAR Card issued by the Unique Identification Authority of India']").show();


                                          /******** HIDE *******/
            $("#ProofIdentity option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
            $("#ProofIdentity option[value='Partnership Deed']").hide();
            $("#ProofIdentity option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
            $("#ProofIdentity option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
            $("#ProofIdentity option[value='Trust Deed']").hide();
            $("#ProofIdentity option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();
        }
        else if ($(this).val() == "Firm") {
            /******** SHOW *******/
            $("#ProofIdentity option[value='Certificate Of Registration Issued By Registrar Of Firms']").show();
            $("#ProofIdentity option[value='Partnership Deed']").show();
            /******** HIDE *******/
            $("#ProofIdentity option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Member of Parliament']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
            $("#ProofIdentity option[value='Driving License']").hide();
            $("#ProofIdentity option[value='Passport']").hide();
            $("#ProofIdentity option[value='Arms license']").hide();
            $("#ProofIdentity option[value='Central Government Health Scheme Card']").hide();
            $("#ProofIdentity option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
            $("#ProofIdentity option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
            $("#ProofIdentity option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
            $("#ProofIdentity option[value='Pensioner Card having photograph of the applicant']").hide();
            $("#ProofIdentity option[value='Electors photo identity card']").hide();
            $("#ProofIdentity option[value='Ration card having photograph of the applicant']").hide();
            $("#ProofIdentity option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();
            $("#ProofIdentity option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
            $("#ProofIdentity option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
            $("#ProofIdentity option[value='Trust Deed']").hide();
            $("#ProofIdentity option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();

        }
        else if ($(this).val() == "Company") {
            /******** SHOW *******/
            $("#ProofIdentity option[value='Certificate of Registration Issue By Resiastar Of Companies']").show();
            /******** HIDE *******/
            $("#ProofIdentity option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
            $("#ProofIdentity option[value='Partnership Deed']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Member of Parliament']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
            $("#ProofIdentity option[value='Driving License']").hide();
            $("#ProofIdentity option[value='Passport']").hide();
            $("#ProofIdentity option[value='Arms license']").hide();
            $("#ProofIdentity option[value='Central Government Health Scheme Card']").hide();
            $("#ProofIdentity option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
            $("#ProofIdentity option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
            $("#ProofIdentity option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
            $("#ProofIdentity option[value='Pensioner Card having photograph of the applicant']").hide();
            $("#ProofIdentity option[value='Electors photo identity card']").hide();
            $("#ProofIdentity option[value='Ration card having photograph of the applicant']").hide();
            $("#ProofIdentity option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();
            
            $("#ProofIdentity option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
            $("#ProofIdentity option[value='Trust Deed']").hide();
            $("#ProofIdentity option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();

        }
        else if ($(this).val() == "Trust") {
            /******** SHOW *******/
            $("#ProofIdentity option[value='Certificate of Registration Number Issued By Charity Commissioner']").show();
            $("#ProofIdentity option[value='Trust Deed']").show();
            /******** HIDE *******/
            $("#ProofIdentity option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
            $("#ProofIdentity option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
            $("#ProofIdentity option[value='Partnership Deed']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Member of Parliament']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
            $("#ProofIdentity option[value='Driving License']").hide();
            $("#ProofIdentity option[value='Passport']").hide();
            $("#ProofIdentity option[value='Arms license']").hide();
            $("#ProofIdentity option[value='Central Government Health Scheme Card']").hide();
            $("#ProofIdentity option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
            $("#ProofIdentity option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
            $("#ProofIdentity option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
            $("#ProofIdentity option[value='Pensioner Card having photograph of the applicant']").hide();
            $("#ProofIdentity option[value='Electors photo identity card']").hide();
            $("#ProofIdentity option[value='Ration card having photograph of the applicant']").hide();
            $("#ProofIdentity option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();

           
            $("#ProofIdentity option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();

        }
        else if ($(this).val() == "Limited Liability Partnership") {
            /******** SHOW *******/
            $("#ProofIdentity option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").show();
            /******** HIDE *******/
            $("#ProofIdentity option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
            $("#ProofIdentity option[value='Trust Deed']").hide();
            $("#ProofIdentity option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
            $("#ProofIdentity option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
            $("#ProofIdentity option[value='Partnership Deed']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Member of Parliament']").hide();
            $("#ProofIdentity option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
            $("#ProofIdentity option[value='Driving License']").hide();
            $("#ProofIdentity option[value='Passport']").hide();
            $("#ProofIdentity option[value='Arms license']").hide();
            $("#ProofIdentity option[value='Central Government Health Scheme Card']").hide();
            $("#ProofIdentity option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
            $("#ProofIdentity option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
            $("#ProofIdentity option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
            $("#ProofIdentity option[value='Pensioner Card having photograph of the applicant']").hide();
            $("#ProofIdentity option[value='Electors photo identity card']").hide();
            $("#ProofIdentity option[value='Ration card having photograph of the applicant']").hide();
            $("#ProofIdentity option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();
        }
    });
    /**************** Category Change event for Address********/
    $("#Category").change(function () {
       

        /**************** Bind Proof of Identitydrop down options  (id = ProofIdentity)********/
        //alert("filter " + fileter);
        if ($(this).val() == "Individual" || $(this).val() == "Body of Individuals" || $(this).val() == "Hindu Undivided Family" || $(this).val() == "Association of Persons" || $(this).val() == "Local Authority" || $(this).val() == "Artificial Juridical Person" || $(this).val() == "Government") {
            /******** SHOW *******/
            $("#ProofAddress option[value='Certificate of Identity signed by a Gazetted Officer']").show();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Legislative Assembly']").show();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Parliament']").show();
            $("#ProofAddress option[value='Certificate of Identity signed by a Municipal Councillor']").show();
            $("#ProofAddress option[value='Driving License']").show();
            $("#ProofAddress option[value='Passport']").show();
            $("#ProofAddress option[value='Arms license']").show();
            $("#ProofAddress option[value='Central Government Health Scheme Card']").show();
            $("#ProofAddress option[value='Ex-Servicemen Contributory Health Scheme photo card']").show();
            $("#ProofAddress option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").show();
            $("#ProofAddress option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").show();
            $("#ProofAddress option[value='Pensioner Card having photograph of the applicant']").show();
            $("#ProofAddress option[value='Electors photo identity card']").show();
            $("#ProofAddress option[value='Ration card having photograph of the applicant']").show();
            $("#ProofAddress option[value='AADHAAR Card issued by the Unique Identification Authority of India']").show();


            /******** HIDE *******/
            $("#ProofAddress option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
            $("#ProofAddress option[value='Partnership Deed']").hide();
            $("#ProofAddress option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
            $("#ProofAddress option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
            $("#ProofAddress option[value='Trust Deed']").hide();
            $("#ProofAddress option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();
        }
        else if ($(this).val() == "Firm") {
            /******** SHOW *******/
            $("#ProofAddress option[value='Certificate Of Registration Issued By Registrar Of Firms']").show();
            $("#ProofAddress option[value='Partnership Deed']").show();
            /******** HIDE *******/
            $("#ProofAddress option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Parliament']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
            $("#ProofAddress option[value='Driving License']").hide();
            $("#ProofAddress option[value='Passport']").hide();
            $("#ProofAddress option[value='Arms license']").hide();
            $("#ProofAddress option[value='Central Government Health Scheme Card']").hide();
            $("#ProofAddress option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
            $("#ProofAddress option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
            $("#ProofAddress option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
            $("#ProofAddress option[value='Pensioner Card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='Electors photo identity card']").hide();
            $("#ProofAddress option[value='Ration card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();
            $("#ProofAddress option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
            $("#ProofAddress option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
            $("#ProofAddress option[value='Trust Deed']").hide();
            $("#ProofAddress option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();

        }
        else if ($(this).val() == "Company") {
            /******** SHOW *******/
            $("#ProofAddress option[value='Certificate of Registration Issue By Resiastar Of Companies']").show();
            /******** HIDE *******/
            $("#ProofAddress option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
            $("#ProofAddress option[value='Partnership Deed']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Parliament']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
            $("#ProofAddress option[value='Driving License']").hide();
            $("#ProofAddress option[value='Passport']").hide();
            $("#ProofAddress option[value='Arms license']").hide();
            $("#ProofAddress option[value='Central Government Health Scheme Card']").hide();
            $("#ProofAddress option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
            $("#ProofAddress option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
            $("#ProofAddress option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
            $("#ProofAddress option[value='Pensioner Card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='Electors photo identity card']").hide();
            $("#ProofAddress option[value='Ration card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();

            $("#ProofAddress option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
            $("#ProofAddress option[value='Trust Deed']").hide();
            $("#ProofAddress option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();

        }
        else if ($(this).val() == "Trust") {
            /******** SHOW *******/
            $("#ProofAddress option[value='Certificate of Registration Number Issued By Charity Commissioner']").show();
            $("#ProofAddress option[value='Trust Deed']").show();
            /******** HIDE *******/
            $("#ProofAddress option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
            $("#ProofAddress option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
            $("#ProofAddress option[value='Partnership Deed']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Parliament']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
            $("#ProofAddress option[value='Driving License']").hide();
            $("#ProofAddress option[value='Passport']").hide();
            $("#ProofAddress option[value='Arms license']").hide();
            $("#ProofAddress option[value='Central Government Health Scheme Card']").hide();
            $("#ProofAddress option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
            $("#ProofAddress option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
            $("#ProofAddress option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
            $("#ProofAddress option[value='Pensioner Card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='Electors photo identity card']").hide();
            $("#ProofAddress option[value='Ration card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();


            $("#ProofAddress option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();

        }
        else if ($(this).val() == "Limited Liability Partnership") {
            /******** SHOW *******/
            $("#ProofAddress option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").show();
            /******** HIDE *******/
            $("#ProofAddress option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
            $("#ProofAddress option[value='Trust Deed']").hide();
            $("#ProofAddress option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
            $("#ProofAddress option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
            $("#ProofAddress option[value='Partnership Deed']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Parliament']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
            $("#ProofAddress option[value='Driving License']").hide();
            $("#ProofAddress option[value='Passport']").hide();
            $("#ProofAddress option[value='Arms license']").hide();
            $("#ProofAddress option[value='Central Government Health Scheme Card']").hide();
            $("#ProofAddress option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
            $("#ProofAddress option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
            $("#ProofAddress option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
            $("#ProofAddress option[value='Pensioner Card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='Electors photo identity card']").hide();
            $("#ProofAddress option[value='Ration card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();
        }
    });
    /**************** Category Change event for DOB********/
    $("#Category").change(function () {


        /**************** Bind Proof of Identitydrop down options  (id = ProofIdentity)********/
        //alert("filter " + fileter);
        if ($(this).val() == "Individual" || $(this).val() == "Body of Individuals" || $(this).val() == "Hindu Undivided Family" || $(this).val() == "Association of Persons" || $(this).val() == "Local Authority" || $(this).val() == "Artificial Juridical Person" || $(this).val() == "Government") {
            /******** SHOW *******/
            $("#ProofAddress option[value='Certificate of Identity signed by a Gazetted Officer']").show();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Legislative Assembly']").show();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Parliament']").show();
            $("#ProofAddress option[value='Certificate of Identity signed by a Municipal Councillor']").show();
            $("#ProofAddress option[value='Driving License']").show();
            $("#ProofAddress option[value='Passport']").show();
            $("#ProofAddress option[value='Arms license']").show();
            $("#ProofAddress option[value='Central Government Health Scheme Card']").show();
            $("#ProofAddress option[value='Ex-Servicemen Contributory Health Scheme photo card']").show();
            $("#ProofAddress option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").show();
            $("#ProofAddress option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").show();
            $("#ProofAddress option[value='Pensioner Card having photograph of the applicant']").show();
            $("#ProofAddress option[value='Electors photo identity card']").show();
            $("#ProofAddress option[value='Ration card having photograph of the applicant']").show();
            $("#ProofAddress option[value='AADHAAR Card issued by the Unique Identification Authority of India']").show();


            /******** HIDE *******/
            $("#ProofAddress option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
            $("#ProofAddress option[value='Partnership Deed']").hide();
            $("#ProofAddress option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
            $("#ProofAddress option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
            $("#ProofAddress option[value='Trust Deed']").hide();
            $("#ProofAddress option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();
        }
        else if ($(this).val() == "Firm") {
            /******** SHOW *******/
            $("#ProofAddress option[value='Certificate Of Registration Issued By Registrar Of Firms']").show();
            $("#ProofAddress option[value='Partnership Deed']").show();
            /******** HIDE *******/
            $("#ProofAddress option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Parliament']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
            $("#ProofAddress option[value='Driving License']").hide();
            $("#ProofAddress option[value='Passport']").hide();
            $("#ProofAddress option[value='Arms license']").hide();
            $("#ProofAddress option[value='Central Government Health Scheme Card']").hide();
            $("#ProofAddress option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
            $("#ProofAddress option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
            $("#ProofAddress option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
            $("#ProofAddress option[value='Pensioner Card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='Electors photo identity card']").hide();
            $("#ProofAddress option[value='Ration card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();
            $("#ProofAddress option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
            $("#ProofAddress option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
            $("#ProofAddress option[value='Trust Deed']").hide();
            $("#ProofAddress option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();

        }
        else if ($(this).val() == "Company") {
            /******** SHOW *******/
            $("#ProofAddress option[value='Certificate of Registration Issue By Resiastar Of Companies']").show();
            /******** HIDE *******/
            $("#ProofAddress option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
            $("#ProofAddress option[value='Partnership Deed']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Parliament']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
            $("#ProofAddress option[value='Driving License']").hide();
            $("#ProofAddress option[value='Passport']").hide();
            $("#ProofAddress option[value='Arms license']").hide();
            $("#ProofAddress option[value='Central Government Health Scheme Card']").hide();
            $("#ProofAddress option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
            $("#ProofAddress option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
            $("#ProofAddress option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
            $("#ProofAddress option[value='Pensioner Card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='Electors photo identity card']").hide();
            $("#ProofAddress option[value='Ration card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();

            $("#ProofAddress option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
            $("#ProofAddress option[value='Trust Deed']").hide();
            $("#ProofAddress option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();

        }
        else if ($(this).val() == "Trust") {
            /******** SHOW *******/
            $("#ProofAddress option[value='Certificate of Registration Number Issued By Charity Commissioner']").show();
            $("#ProofAddress option[value='Trust Deed']").show();
            /******** HIDE *******/
            $("#ProofAddress option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
            $("#ProofAddress option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
            $("#ProofAddress option[value='Partnership Deed']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Parliament']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
            $("#ProofAddress option[value='Driving License']").hide();
            $("#ProofAddress option[value='Passport']").hide();
            $("#ProofAddress option[value='Arms license']").hide();
            $("#ProofAddress option[value='Central Government Health Scheme Card']").hide();
            $("#ProofAddress option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
            $("#ProofAddress option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
            $("#ProofAddress option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
            $("#ProofAddress option[value='Pensioner Card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='Electors photo identity card']").hide();
            $("#ProofAddress option[value='Ration card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();


            $("#ProofAddress option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").hide();

        }
        else if ($(this).val() == "Limited Liability Partnership") {
            /******** SHOW *******/
            $("#ProofAddress option[value='Copy Of Certificate of Registration Issued By the Registrar Of Firm/Limited Liability Partnerships']").show();
            /******** HIDE *******/
            $("#ProofAddress option[value='Certificate of Registration Number Issued By Charity Commissioner']").hide();
            $("#ProofAddress option[value='Trust Deed']").hide();
            $("#ProofAddress option[value='Certificate of Registration Issue By Resiastar Of Companies']").hide();
            $("#ProofAddress option[value='Certificate Of Registration Issued By Registrar Of Firms']").hide();
            $("#ProofAddress option[value='Partnership Deed']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Gazetted Officer']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Legislative Assembly']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Member of Parliament']").hide();
            $("#ProofAddress option[value='Certificate of Identity signed by a Municipal Councillor']").hide();
            $("#ProofAddress option[value='Driving License']").hide();
            $("#ProofAddress option[value='Passport']").hide();
            $("#ProofAddress option[value='Arms license']").hide();
            $("#ProofAddress option[value='Central Government Health Scheme Card']").hide();
            $("#ProofAddress option[value='Ex-Servicemen Contributory Health Scheme photo card']").hide();
            $("#ProofAddress option[value='Bank certificate in Original on letter head from the branch (along with name and stamp of the issuing officer) containing duly attested photograph and bank account number of the applicant']").hide();
            $("#ProofAddress option[value='Photo identity Card issued by the Central Government or State Government or Public Sector Undertaking']").hide();
            $("#ProofAddress option[value='Pensioner Card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='Electors photo identity card']").hide();
            $("#ProofAddress option[value='Ration card having photograph of the applicant']").hide();
            $("#ProofAddress option[value='AADHAAR Card issued by the Unique Identification Authority of India']").hide();
        }
    });
    /**************** Status Change event********/
    $("#Status").change(function () {
        if ($(this).val() == "Change") {
            $("#PANCARDNO").show();
        } else {
            $("#PANCARDNO").hide();
        }
    });


});

