

/*=============================================
	=    		 Preloader			      =
=============================================*/

window.console = window.console || function(t) {};


/*=============================================
	=    		Counter js Start		      =
=============================================*/
//Counter js Start



    (function ($){
      $.fn.counter = function() {
        const $this = $(this),
        numberFrom = parseInt($this.attr('data-from')),
        numberTo = parseInt($this.attr('data-to')),
        delta = numberTo - numberFrom,
        deltaPositive = delta > 0 ? 1 : 0,
        time = parseInt($this.attr('data-time')),
        changeTime = 10;
      
        let currentNumber = numberFrom,
        value = delta*changeTime/time;
        var interval1;
              const changeNumber = () => {
                currentNumber += value;
                //checks if currentNumber reached numberTo
                (deltaPositive && currentNumber >= numberTo) || (!deltaPositive &&currentNumber<= numberTo) ? currentNumber=numberTo : currentNumber;
                this.text(parseInt(currentNumber));
                currentNumber == numberTo ? clearInterval(interval1) : currentNumber;  
              }
        interval1 = setInterval(changeNumber,changeTime);
      }

      }(jQuery));

      $(document).ready(function(){

      $('.count-up').counter();
      $('.count1').counter();
      $('.count2').counter();
      $('.count3').counter();
    
      new WOW().init();
      
      setTimeout(function () {
        $('.count5').counter();
      }, 3000);
    });


/*=============================================
	=     Menu sticky & Scroll to top      =
=============================================*/

 if (document.location.search.match(/type=embed/gi)) {
        window.parent.postMessage("resize", "*");
      }

/*=============================================
	=    		 Form js Start  	         =
=============================================*/
		function myFunction() {
		var name = document.getElementById("name").value;
		var email = document.getElementById("txtEmail").value;
		var txtmobile = document.getElementById("txtmobile").value;
		var subject = document.getElementById("subject").value;
		var message = document.getElementById("message").value;

		var i=0;
		var aa="";


		if(name=="")
		{                                    
		  i=1;
		  aa=aa+"Please Enter Name ! \n";

		}                                  
		if(email=="")
		{
		  i=1;
		  aa=aa+"Please Enter Email ! \n";
		}
		if(txtmobile=="" || txtmobile=="+91 ")
		{
		  i=1;
		  aa=aa+"Please Enter number ! \n";
		}

		 if(subject=="" || subject=="")
		{
		  i=1;
		  aa=aa+"Please Enter Subject ! \n";
		}
		 if(message=="" || message=="")
		{
		  i=1;
		  aa=aa+"Please Enter Subject ! \n";
		}



		if(i==1)
		{
		  alert(aa);
		}
		}



 /* all Js Start*/















 (function ($) {
	"use strict";

/*=============================================
	=    		 Preloader			      =
=============================================*/
function preloader() {
	$('#preloader').delay(0).fadeOut();
};

$(window).on('load', function () {
	preloader();
	mainSlider();
	aosAnimation();
	wowAnimation();
});



/*=============================================
	=    		Mobile Menu			      =
=============================================*/
//SubMenu Dropdown Toggle
if ($('.menu-area li.menu-item-has-children ul').length) {
	$('.menu-area .navigation li.menu-item-has-children').append('<div class="dropdown-btn"><span class="fas fa-angle-down"></span></div>');

}

//Mobile Nav Hide Show
if ($('.mobile-menu').length) {

	var mobileMenuContent = $('.menu-area .main-menu').html();
	$('.mobile-menu .menu-box .menu-outer').append(mobileMenuContent);

	//Dropdown Button
	$('.mobile-menu li.menu-item-has-children .dropdown-btn').on('click', function () {
		$(this).toggleClass('open');
		$(this).prev('ul').slideToggle(500);
	});
	//Menu Toggle Btn
	$('.mobile-nav-toggler').on('click', function () {
		$('body').addClass('mobile-menu-visible');
	});

	//Menu Toggle Btn
	$('.menu-backdrop, .mobile-menu .close-btn').on('click', function () {
		$('body').removeClass('mobile-menu-visible');
	});
}


/*=============================================
	=     Menu sticky & Scroll to top      =
=============================================*/
$(window).on('scroll', function () {
	var scroll = $(window).scrollTop();
	if (scroll < 245) {
		$("#sticky-header").removeClass("sticky-menu");
		$('.scroll-to-target').removeClass('open');

	} else {
		$("#sticky-header").addClass("sticky-menu");
		$('.scroll-to-target').addClass('open');
	}
});






/*=============================================
	=    		 Aos Active  	         =
=============================================*/
function aosAnimation() {
	AOS.init({
		duration: 1000,
		mirror: true,
		once: true,
		disable: 'mobile',
	});
}



/*=============================================
	=    		 Wow Active  	         =
=============================================*/
function wowAnimation() {
	var wow = new WOW({
		boxClass: 'wow',
		animateClass: 'animated',
		offset: 0,
		mobile: false,
		live: true
	});
	wow.init();
}


})(jQuery);