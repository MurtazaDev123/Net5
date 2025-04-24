/* ========================================== 
scrollTop() >= 44
Should be equal the the height of the header
========================================== */

$(window).scroll(function(){
  if ($(window).scrollTop() >= 44) {
      $('.mainHeader').addClass('fixed-header');
      $('.action').addClass('fix');
  }
  else {
      $('.mainHeader').removeClass('fixed-header');
      $('.action').removeClass('fix');
  }
});

/**
 * main.js
 * http://www.codrops.com
 *
 * Licensed under the MIT license.
 * http://www.opensource.org/licenses/mit-license.php
 * 
 * Copyright 2015, Codrops
 * http://www.codrops.com
 */

;(function(window) {

	'use strict';

	var support = { animations : Modernizr.cssanimations },
		animEndEventNames = { 'WebkitAnimation' : 'webkitAnimationEnd', 'OAnimation' : 'oAnimationEnd', 'msAnimation' : 'MSAnimationEnd', 'animation' : 'animationend' },
		animEndEventName = animEndEventNames[ Modernizr.prefixed( 'animation' ) ],
		onEndAnimation = function( el, callback ) {
			var onEndCallbackFn = function( ev ) {
				if( support.animations ) {
					if( ev.target != this ) return;
					this.removeEventListener( animEndEventName, onEndCallbackFn );
				}
				if( callback && typeof callback === 'function' ) { callback.call(); }
			};
			if( support.animations ) {
				el.addEventListener( animEndEventName, onEndCallbackFn );
			}
			else {
				onEndCallbackFn();
			}
		};

	function extend( a, b ) {
		for( var key in b ) { 
			if( b.hasOwnProperty( key ) ) {
				a[key] = b[key];
			}
		}
		return a;
	}

	function MLMenu(el, options) {
		this.el = el;
		this.options = extend( {}, this.options );
		extend( this.options, options );
		
		// the menus (<ul>´s)
		this.menus = [].slice.call(this.el.querySelectorAll('.menu__level'));

		// index of current menu
		// Each level is actually a different menu so 0 is root, 1 is sub-1, 2 sub-2, etc.
		this.current_menu = 0;

		/* Determine what current menu actually is */
		var current_menu;
		this.menus.forEach(function(menuEl, pos) {
			var items = menuEl.querySelectorAll('.menu__item');
			items.forEach(function(itemEl, iPos) {
				var currentLink = itemEl.querySelector('.menu__link--current');
				if (currentLink) {
					// This is the actual menu__level that should have current
					current_menu = pos;
				}
			});
		});

		if (current_menu) {
			this.current_menu = current_menu;	
		}

		this._init();
	}

	MLMenu.prototype.options = {
		// show breadcrumbs
		breadcrumbsCtrl : true,
		// initial breadcrumb text
		initialBreadcrumb : 'Shop By Category',
		// show back button
		backCtrl : true,
		// delay between each menu item sliding animation
		itemsDelayInterval : 60,
		// direction 
		direction : 'r2l',
		// callback: item that doesn´t have a submenu gets clicked
		// onItemClick([event], [inner HTML of the clicked item])
		onItemClick : function(ev, itemName) { return false; }
	};

	MLMenu.prototype._init = function() {
		// iterate the existing menus and create an array of menus, 
		// more specifically an array of objects where each one holds the info of each menu element and its menu items
		this.menusArr = [];
		this.breadCrumbs = false;
		var self = this;
		var submenus = [];

		/* Loops over root level menu items */
		this.menus.forEach(function(menuEl, pos) {
			var menu = {menuEl : menuEl, menuItems : [].slice.call(menuEl.querySelectorAll('.menu__item'))};
			
			self.menusArr.push(menu);

			// set current menu class
			if( pos === self.current_menu ) {
				classie.add(menuEl, 'menu__level--current');
			}

			var menu_x = menuEl.getAttribute('data-menu');
			var links = menuEl.querySelectorAll('.menu__link');
			links.forEach(function(linkEl, lPos) {
				var submenu = linkEl.getAttribute('data-submenu');
				if (submenu) {
					var pushMe = {"menu":submenu, "name": linkEl.innerHTML };
					if (submenus[pos]) {
						submenus[pos].push(pushMe);
					} else {
						submenus[pos] = []
						submenus[pos].push(pushMe);
					}
				}
			});
		});

		/* For each MENU, find their parent MENU */		
		this.menus.forEach(function(menuEl, pos) {
			var menu_x = menuEl.getAttribute('data-menu');
			submenus.forEach(function(subMenuEl, menu_root) {
				subMenuEl.forEach(function(subMenuItem, subPos) {
					if (subMenuItem.menu == menu_x) {
						self.menusArr[pos].backIdx = menu_root;
						self.menusArr[pos].name = subMenuItem.name;
					}
				});
			});
		});

		// create breadcrumbs
		if( self.options.breadcrumbsCtrl ) {
			this.breadcrumbsCtrl = document.createElement('nav');
			this.breadcrumbsCtrl.className = 'menu__breadcrumbs';
			this.breadcrumbsCtrl.setAttribute('aria-label', 'You are here');
			this.el.insertBefore(this.breadcrumbsCtrl, this.el.firstChild);
			// add initial breadcrumb
			this._addBreadcrumb(0);
			
			// Need to add breadcrumbs for all parents of current submenu
			if (self.menusArr[self.current_menu].backIdx != 0 && self.current_menu != 0) {
				this._crawlCrumbs(self.menusArr[self.current_menu].backIdx, self.menusArr);
				this.breadCrumbs = true;
			}

			// Create current submenu breadcrumb
			if (self.current_menu != 0) {
				this._addBreadcrumb(self.current_menu);
				this.breadCrumbs = true;
			}
		}

		// create back button
		if (this.options.backCtrl) {
			this.backCtrl = document.createElement('button');
			if (this.breadCrumbs) {
				this.backCtrl.className = 'menu__back';	
			} else {
				this.backCtrl.className = 'menu__back menu__back--hidden';
			}
			this.backCtrl.setAttribute('aria-label', 'Go back');
			this.backCtrl.innerHTML = '<span class="icon icon--arrow-left"></span>';
			this.el.insertBefore(this.backCtrl, this.el.firstChild);
		}

		// event binding
		this._initEvents();
	};

	MLMenu.prototype._initEvents = function() {
		var self = this;

		for(var i = 0, len = this.menusArr.length; i < len; ++i) {
			this.menusArr[i].menuItems.forEach(function(item, pos) {
				item.querySelector('a').addEventListener('click', function(ev) { 
					var submenu = ev.target.getAttribute('data-submenu'),
						itemName = ev.target.innerHTML,
						subMenuEl = self.el.querySelector('ul[data-menu="' + submenu + '"]');

					// check if there's a sub menu for this item
					if( submenu && subMenuEl ) {
						ev.preventDefault();
						// open it
						self._openSubMenu(subMenuEl, pos, itemName);
					}
					else {
						// add class current
						var currentlink = self.el.querySelector('.menu__link--current');
						if( currentlink ) {
							classie.remove(self.el.querySelector('.menu__link--current'), 'menu__link--current');
						}
						classie.add(ev.target, 'menu__link--current');
						
						// callback
						self.options.onItemClick(ev, itemName);
					}
				});
			});
		}
		
		// back navigation
		if( this.options.backCtrl ) {
			this.backCtrl.addEventListener('click', function() {
				self._back();
			});
		}
	};

	MLMenu.prototype._openSubMenu = function(subMenuEl, clickPosition, subMenuName) {
		if( this.isAnimating ) {
			return false;
		}
		this.isAnimating = true;
		
		// save "parent" menu index for back navigation
		this.menusArr[this.menus.indexOf(subMenuEl)].backIdx = this.current_menu;
		// save "parent" menu´s name
		this.menusArr[this.menus.indexOf(subMenuEl)].name = subMenuName;
		// current menu slides out
		this._menuOut(clickPosition);
		// next menu (submenu) slides in
		this._menuIn(subMenuEl, clickPosition);
	};

	MLMenu.prototype._back = function() {
		if( this.isAnimating ) {
			return false;
		}
		this.isAnimating = true;

		// current menu slides out
		this._menuOut();
		// next menu (previous menu) slides in
		var backMenu = this.menusArr[this.menusArr[this.current_menu].backIdx].menuEl;
		this._menuIn(backMenu);

		// remove last breadcrumb
		if( this.options.breadcrumbsCtrl ) {
			this.breadcrumbsCtrl.removeChild(this.breadcrumbsCtrl.lastElementChild);
		}
	};

	MLMenu.prototype._menuOut = function(clickPosition) {
		// the current menu
		var self = this,
			currentMenu = this.menusArr[this.current_menu].menuEl,
			isBackNavigation = typeof clickPosition == 'undefined' ? true : false;

		// slide out current menu items - first, set the delays for the items
		this.menusArr[this.current_menu].menuItems.forEach(function(item, pos) {
			item.style.WebkitAnimationDelay = item.style.animationDelay = isBackNavigation ? parseInt(pos * self.options.itemsDelayInterval) + 'ms' : parseInt(Math.abs(clickPosition - pos) * self.options.itemsDelayInterval) + 'ms';
		});
		// animation class
		if( this.options.direction === 'r2l' ) {
			classie.add(currentMenu, !isBackNavigation ? 'animate-outToLeft' : 'animate-outToRight');
		}
		else {
			classie.add(currentMenu, isBackNavigation ? 'animate-outToLeft' : 'animate-outToRight');	
		}
	};

	MLMenu.prototype._menuIn = function(nextMenuEl, clickPosition) {
		var self = this,
			// the current menu
			currentMenu = this.menusArr[this.current_menu].menuEl,
			isBackNavigation = typeof clickPosition == 'undefined' ? true : false,
			// index of the nextMenuEl
			nextMenuIdx = this.menus.indexOf(nextMenuEl),

			nextMenu = this.menusArr[nextMenuIdx],
			nextMenuEl = nextMenu.menuEl,
			nextMenuItems = nextMenu.menuItems,
			nextMenuItemsTotal = nextMenuItems.length;

		// slide in next menu items - first, set the delays for the items
		nextMenuItems.forEach(function(item, pos) {
			item.style.WebkitAnimationDelay = item.style.animationDelay = isBackNavigation ? parseInt(pos * self.options.itemsDelayInterval) + 'ms' : parseInt(Math.abs(clickPosition - pos) * self.options.itemsDelayInterval) + 'ms';

			// we need to reset the classes once the last item animates in
			// the "last item" is the farthest from the clicked item
			// let's calculate the index of the farthest item
			var farthestIdx = clickPosition <= nextMenuItemsTotal/2 || isBackNavigation ? nextMenuItemsTotal - 1 : 0;

			if( pos === farthestIdx ) {
				onEndAnimation(item, function() {
					// reset classes
					if( self.options.direction === 'r2l' ) {
						classie.remove(currentMenu, !isBackNavigation ? 'animate-outToLeft' : 'animate-outToRight');
						classie.remove(nextMenuEl, !isBackNavigation ? 'animate-inFromRight' : 'animate-inFromLeft');
					}
					else {
						classie.remove(currentMenu, isBackNavigation ? 'animate-outToLeft' : 'animate-outToRight');
						classie.remove(nextMenuEl, isBackNavigation ? 'animate-inFromRight' : 'animate-inFromLeft');
					}
					classie.remove(currentMenu, 'menu__level--current');
					classie.add(nextMenuEl, 'menu__level--current');

					//reset current
					self.current_menu = nextMenuIdx;

					// control back button and breadcrumbs navigation elements
					if( !isBackNavigation ) {
						// show back button
						if( self.options.backCtrl ) {
							classie.remove(self.backCtrl, 'menu__back--hidden');
						}
						
						// add breadcrumb
						self._addBreadcrumb(nextMenuIdx);
					}
					else if( self.current_menu === 0 && self.options.backCtrl ) {
						// hide back button
						classie.add(self.backCtrl, 'menu__back--hidden');
					}

					// we can navigate again..
					self.isAnimating = false;

					// focus retention
					nextMenuEl.focus();
				});
			}
		});

		// animation class
		if( this.options.direction === 'r2l' ) {
			classie.add(nextMenuEl, !isBackNavigation ? 'animate-inFromRight' : 'animate-inFromLeft');
		}
		else {
			classie.add(nextMenuEl, isBackNavigation ? 'animate-inFromRight' : 'animate-inFromLeft');
		}
	};

	MLMenu.prototype._addBreadcrumb = function(idx) {
		if( !this.options.breadcrumbsCtrl ) {
			return false;
		}

		var bc = document.createElement('a');
		bc.href = '#'; // make it focusable
		bc.innerHTML = idx ? this.menusArr[idx].name : this.options.initialBreadcrumb;
		this.breadcrumbsCtrl.appendChild(bc);

		var self = this;
		bc.addEventListener('click', function(ev) {
			ev.preventDefault();

			// do nothing if this breadcrumb is the last one in the list of breadcrumbs
			if( !bc.nextSibling || self.isAnimating ) {
				return false;
			}
			self.isAnimating = true;
			
			// current menu slides out
			self._menuOut();
			// next menu slides in
			var nextMenu = self.menusArr[idx].menuEl;
			self._menuIn(nextMenu);

			// remove breadcrumbs that are ahead
			var siblingNode;
			while (siblingNode = bc.nextSibling) {
				self.breadcrumbsCtrl.removeChild(siblingNode);
			}
		});
	};

	MLMenu.prototype._crawlCrumbs = function(currentMenu, menuArray) {
		if (menuArray[currentMenu].backIdx != 0) {
			this._crawlCrumbs(menuArray[currentMenu].backIdx, menuArray);
		}
		// create breadcrumb
		this._addBreadcrumb(currentMenu);
	}

	window.MLMenu = MLMenu;

})(window);
  
  $('.selectpicker').selectpicker();

  $(document).ready(function(e){
    $('.search-panel .dropdown-menu').find('a').click(function(e) {
		e.preventDefault();
		var param = $(this).attr("href").replace("#","");
		var concept = $(this).text();
		$('.search-panel span#search_concept').text(concept);
		$('.input-group #search_param').val(param);
	});
});
  
  $('.rating input').change(function () {
    var $radio = $(this);
    $('.rating .selected').removeClass('selected');
    $radio.closest('label').addClass('selected');
  });

  
  $('.centerSlider').slick({
    centerMode: true,
    slidesToShow: 1,
    dots:true,
    arrows: false,
    responsive: [
      {
        breakpoint: 1600,
        settings: {
          centerPadding: '230px',
        }
      },
      {
        breakpoint: 1430,
        settings: {
          centerPadding: '180px',
        }
      },
      {
        breakpoint: 1366,
        settings: {
          centerPadding: '150px',
        }
      },
      {
        breakpoint: 1250,
        settings: {
          centerPadding: '100px',
        }
      },
      {
        breakpoint: 1150,
        settings: {
          centerPadding: '70px',
        }
      },
      {
        breakpoint: 1080,
        settings: {
          centerMode: false,
          centerPadding: '50px',
          arrows: false,

        }
      }
      // You can unslick at a given breakpoint now by adding:
      // settings: "unslick"
      // instead of a settings object
    ]
  });

  $('.popularCatInner').slick({
    slidesToShow: 6,
    Margin: 0,
    dots:false,
    responsive: [
      {
        breakpoint: 1600,
      },
      {
        breakpoint: 1500,
        settings: {
          slidesToShow: 5,
        }
      },
      {
        breakpoint: 1400,
        settings: {
          slidesToShow: 5,
        }
      },
      {
        breakpoint: 1300,
        settings: {
          slidesToShow: 5,
        }
      },
      {
        breakpoint: 1200,
        settings: {
          slidesToShow: 5,
        }
      },
      {
        breakpoint: 992,
        settings: {
          slidesToShow: 4,
        }
      },
      {
        breakpoint: 786,
        settings: {
          slidesToShow: 3,
          arrows: false,
        }
      },
      {
        breakpoint: 520,
        settings: {
          slidesToShow: 2,
          arrows: false,
        }
      }
    ]
  });



  $('.fiveSlider').slick({
    slidesToShow: 6,
    Margin: 0,
    dots:false,
    responsive: [
      {
        breakpoint: 1600,
      },
      {
        breakpoint: 1500,
        settings: {
          slidesToShow: 5,
        }
      },
      {
        breakpoint: 1400,
        settings: {
          slidesToShow: 5,
        }
      },
      {
        breakpoint: 1300,
        settings: {
          slidesToShow: 5,
        }
      },
      {
        breakpoint: 1200,
        settings: {
          slidesToShow: 4,
        }
      },
      {
        breakpoint: 992,
        settings: {
          slidesToShow: 3,
        }
      },
      {
        breakpoint: 786,
        settings: {
          slidesToShow: 2,
          arrows: false,
        }
      }
    ]
  });

  $('.fourSlider').slick({
    slidesToShow: 5,
    Margin: 10,
    dots:false,
    responsive: [
      {
        breakpoint: 1600,
      },
      {
        breakpoint: 1500,
        settings: {
          slidesToShow: 4,
        }
      },
      {
        breakpoint: 1400,
        settings: {
          slidesToShow: 4,
        }
      },
      {
        breakpoint: 1300,
        settings: {
          slidesToShow: 4,
        }
      },
      {
        breakpoint: 1200,
        settings: {
          slidesToShow: 3,
        }
      },
      {
        breakpoint: 992,
        settings: {
          slidesToShow: 2,
        }
      },
      {
        breakpoint: 786,
        settings: {
          slidesToShow: 1,
          arrows: false,
        }
      }
    ]
  });

    
  $('.bannerSliderInner').slick({
    slidesToShow: 3,
    centerPadding: '30px',
    dots:false,
    arrows: false,
    variableWidth: true
  });

  $('.sliderDescriptionInner').slick({
    slidesToShow: 3,
    dots:false,
    responsive: [
      {
        breakpoint: 1200,
        settings: {
          slidesToShow: 2,
        }
      },
      {
        breakpoint: 992,
        settings: {
          slidesToShow: 2,
        }
      },
      {
        breakpoint: 786,
        settings: {
          slidesToShow: 1,
          arrows: false,
        }
      }
    ]
  });

  /* Testimonial Slider */

  $('.slider-for').slick({
    slidesToShow: 1,
    slidesToScroll: 1,
    arrows: false,
    fade: true,
    asNavFor: '.slider-nav'
  });
  $('.slider-nav').slick({
    slidesToShow: 5,
    slidesToScroll: 1,
    asNavFor: '.slider-for',
    dots: false,
    arrows: false,
    centerMode: true,
    focusOnSelect: true,
    centerPadding: '0px',
    Margin: '0px',
  });
    
  /* End Testimonial Slider */


    

  $('.searchSlider').slick({
    slidesToShow: 1,
    dots:true,
    arrows: false,
    responsive: [
      {
        breakpoint: 1600,
        settings: {
          centerPadding: '230px',
        }
      },
      {
        breakpoint: 1430,
        settings: {
          centerPadding: '180px',
        }
      },
      {
        breakpoint: 1366,
        settings: {
          centerPadding: '150px',
        }
      },
      {
        breakpoint: 1250,
        settings: {
          centerPadding: '100px',
        }
      },
      {
        breakpoint: 1150,
        settings: {
          centerPadding: '70px',
        }
      },
      {
        breakpoint: 1080,
        settings: {
          centerMode: false,
          centerPadding: '50px',
        }
      }
      // You can unslick at a given breakpoint now by adding:
      // settings: "unslick"
      // instead of a settings object
    ]
  });

  
  

  (function() {
 
    window.inputNumber = function(el) {
  
      var min = el.attr('min') || false;
      var max = el.attr('max') || false;
  
      var els = {};
  
      els.dec = el.prev();
      els.inc = el.next();
  
      el.each(function() {
        init($(this));
      });
  
      function init(el) {
  
        els.dec.on('click', decrement);
        els.inc.on('click', increment);
  
        function decrement() {
          var value = el[0].value;
          value--;
          if(!min || value >= min) {
            el[0].value = value;
          }
        }
  
        function increment() {
          var value = el[0].value;
          value++;
          if(!max || value <= max) {
            el[0].value = value++;
          }
        }
      }
    }
  })();
  
  inputNumber($('.input-number'));



  jQuery(document).ready(function($) {
      var alterClass = function() {
        var ww = document.body.clientWidth;
         
        if (ww > 1290) {
          $('.nonCarousel .productBox').addClass('BoxGrid6');
        } else {
          $('.nonCarousel .productBox').removeClass('BoxGrid6');
        };
        if (ww > 1199) {
          $('.nonCarousel .productBox').addClass('BoxGrid5');
        } else {
          $('.nonCarousel .productBox').removeClass('BoxGrid5');
        };
        if (ww > 991) {
          $('.nonCarousel .productBox').addClass('BoxGrid4');
        } else {
          $('.nonCarousel .productBox').removeClass('BoxGrid4');
        };
        if (ww > 767) {
          $('.nonCarousel .productBox').addClass('BoxGrid3');
        } else {
          $('.nonCarousel .productBox').removeClass('BoxGrid3');
        };
        if (ww < 767) {
          $('.nonCarousel .productBox').addClass('BoxGrid2');
        } else {
          $('.nonCarousel .productBox').removeClass('BoxGrid2');
        };
    };
    $(window).resize(function(){
      alterClass();
    });
    //Fire it when the page first loads:
    alterClass();
  });

  jQuery(document).ready(function($) {
      var alterClass = function() {
        var ww = document.body.clientWidth;
         
        if (ww > 1290) {
          $('.nonCarouselShop .malls ,.nonCarouselShop .shop').addClass('BoxGrid5');
        } else {
          $('.nonCarouselShop .malls ,.nonCarouselShop .shop').removeClass('BoxGrid5');
        };
        if (ww > 1199) {
          $('.nonCarouselShop .malls ,.nonCarouselShop .shop').addClass('BoxGrid4');
        } else {
          $('.nonCarouselShop .malls ,.nonCarouselShop .shop').removeClass('BoxGrid4');
        };
        if (ww > 991) {
          $('.nonCarouselShop .malls ,.nonCarouselShop .shop').addClass('BoxGrid3');
        } else {
          $('.nonCarouselShop .malls ,.nonCarouselShop .shop').removeClass('BoxGrid3');
        };
        if (ww > 767) {
          $('.nonCarouselShop .malls ,.nonCarouselShop .shop').addClass('BoxGrid3');
        } else {
          $('.nonCarouselShop .malls ,.nonCarouselShop .shop').removeClass('BoxGrid3');
        };
        if (ww > 450) {
            $('.nonCarouselShop .malls ,.nonCarouselShop .shop').addClass('BoxGrid2');
          } else {
            $('.nonCarouselShop .malls ,.nonCarouselShop .shop').removeClass('BoxGrid2');
          };
      if (ww < 450) {
              $('.nonCarouselShop .malls ,.nonCarouselShop .shop').addClass('BoxGrid1');
            } else {
              $('.nonCarouselShop .malls ,.nonCarouselShop .shop').removeClass('BoxGrid1');
            };
        
    };
    $(window).resize(function(){
      alterClass();
    });
    //Fire it when the page first loads:
    alterClass();
  });

  jQuery(document).ready(function($) {
      var alterClass = function() {
        var ww = document.body.clientWidth;
         
        if (ww > 1290) {
          $('.searchGridBox .productBox').addClass('BoxGrid5');
        } else {
          $('.searchGridBox .productBox').removeClass('BoxGrid5');
        };
        if (ww > 1199) {
          $('.searchGridBox .productBox').addClass('BoxGrid4');
        } else {
          $('.searchGridBox .productBox').removeClass('BoxGrid4');
        };
        if (ww > 991) {
          $('.searchGridBox .productBox').addClass('BoxGrid4');
        } else {
          $('.searchGridBox .productBox').removeClass('BoxGrid4');
        };
        if (ww > 767) {
          $('.searchGridBox .productBox').addClass('BoxGrid3');
        } else {
          $('.searchGridBox .productBox').removeClass('BoxGrid3');
        };
        if (ww < 767) {
          $('.searchGridBox .productBox').addClass('BoxGrid2');
        } else {
          $('.searchGridBox .productBox').removeClass('BoxGrid2');
        };
    };
    $(window).resize(function(){
      alterClass();
    });
    //Fire it when the page first loads:
    alterClass();
  });
    
  (function() {
    var menuEl = document.getElementById('ml-menu'),
      mlmenu = new MLMenu(menuEl, {
        // breadcrumbsCtrl : true, // show breadcrumbs
        // initialBreadcrumb : 'all', // initial breadcrumb text
        backCtrl : false, // show back button
        // itemsDelayInterval : 60, // delay between each menu item sliding animation
        onItemClick: loadDummyData // callback: item that doesn´t have a submenu gets clicked - onItemClick([event], [inner HTML of the clicked item])
      });

    // mobile menu toggle
    var openMenuCtrl = document.querySelector('.action--open'),
      closeMenuCtrl = document.querySelector('.action--close');

    openMenuCtrl.addEventListener('click', openMenu);
    closeMenuCtrl.addEventListener('click', closeMenu);

    function openMenu() {
      classie.add(menuEl, 'menu--open');
      closeMenuCtrl.focus();
    }

    function closeMenu() {
      classie.remove(menuEl, 'menu--open');
      openMenuCtrl.focus();
    }

    // simulate grid content loading

    function loadDummyData(ev, itemName) {
      ev.preventDefault();

      closeMenu();
      gridWrapper.innerHTML = '';
      classie.add(gridWrapper, 'content--loading');
      setTimeout(function() {
        classie.remove(gridWrapper, 'content--loading');
        gridWrapper.innerHTML = '<ul class="products">' + dummyData[itemName] + '<ul>';
      }, 700);
    }
  })();



  /*
 *  jQuery StarRatingSvg v1.2.0
 *
 *  http://github.com/nashio/star-rating-svg
 *  Author: Ignacio Chavez
 *  hello@ignaciochavez.com
 *  Licensed under MIT
 */

;(function ( $, window, document, undefined ) {

  'use strict';

  // Create the defaults once
  var pluginName = 'starRating';
  var noop = function(){};
  var defaults = {
    totalStars: 5,
    useFullStars: false,
    starShape: 'straight',
    emptyColor: 'lightgray',
    hoverColor: 'orange',
    activeColor: 'gold',
    ratedColor: 'crimson',
    useGradient: true,
    readOnly: false,
    disableAfterRate: true,
    baseUrl: false,
    starGradient: {
      start: '#FEF7CD',
      end: '#FF9511'
    },
    strokeWidth: 4,
    strokeColor: 'black',
    initialRating: 0,
    starSize: 40,
    callback: noop,
    onHover: noop,
    onLeave: noop
  };

	// The actual plugin constructor
  var Plugin = function( element, options ) {
    var _rating;
    var newRating;
    var roundFn;

    this.element = element;
    this.$el = $(element);
    this.settings = $.extend( {}, defaults, options );

    // grab rating if defined on the element
    _rating = this.$el.data('rating') || this.settings.initialRating;

    // round to the nearest half
    roundFn = this.settings.forceRoundUp ? Math.ceil : Math.round;
    newRating = (roundFn( _rating * 2 ) / 2).toFixed(1);
    this._state = {
      rating: newRating
    };

    // create unique id for stars
    this._uid = Math.floor( Math.random() * 999 );

    // override gradient if not used
    if( !options.starGradient && !this.settings.useGradient ){
      this.settings.starGradient.start = this.settings.starGradient.end = this.settings.activeColor;
    }

    this._defaults = defaults;
    this._name = pluginName;
    this.init();
  };

  var methods = {
    init: function () {
      this.renderMarkup();
      this.addListeners();
      this.initRating();
    },

    addListeners: function(){
      if( this.settings.readOnly ){ return; }
      this.$stars.on('mouseover', this.hoverRating.bind(this));
      this.$stars.on('mouseout', this.restoreState.bind(this));
      this.$stars.on('click', this.handleRating.bind(this));
    },

    // apply styles to hovered stars
    hoverRating: function(e){
      var index = this.getIndex(e);
      this.paintStars(index, 'hovered');
      this.settings.onHover(index + 1, this._state.rating, this.$el);
    },

    // clicked on a rate, apply style and state
    handleRating: function(e){
      var index = this.getIndex(e);
      var rating = index + 1;

      this.applyRating(rating, this.$el);
      this.executeCallback( rating, this.$el );

      if(this.settings.disableAfterRate){
        this.$stars.off();
      }
    },

    applyRating: function(rating){
      var index = rating - 1;
      // paint selected and remove hovered color
      this.paintStars(index, 'rated');
      this._state.rating = index + 1;
      this._state.rated = true;
    },

    restoreState: function(e){
      var index = this.getIndex(e);
      var rating = this._state.rating || -1;
      // determine star color depending on manually rated
      var colorType = this._state.rated ? 'rated' : 'active';
      this.paintStars(rating - 1, colorType);
      this.settings.onLeave(index + 1, this._state.rating, this.$el);
    },

    getIndex: function(e){
      var $target = $(e.currentTarget);
      var width = $target.width();
      var side = $(e.target).attr('data-side');
      var minRating = this.settings.minRating;

      // hovered outside the star, calculate by pixel instead
      side = (!side) ? this.getOffsetByPixel(e, $target, width) : side;
      side = (this.settings.useFullStars) ? 'right' : side ;

      // get index for half or whole star
      var index = $target.index() - ((side === 'left') ? 0.5 : 0);

      // pointer is way to the left, rating should be none
      index = ( index < 0.5 && (e.offsetX < width / 4) ) ? -1 : index;

      // force minimum rating
      index = ( minRating && minRating <= this.settings.totalStars && index < minRating ) ? minRating - 1 : index;
      return index;
    },

    getOffsetByPixel: function(e, $target, width){
      var leftX = e.pageX - $target.offset().left;
      return ( leftX <= (width / 2) && !this.settings.useFullStars) ? 'left' : 'right';
    },

    initRating: function(){
      this.paintStars(this._state.rating - 1, 'active');
    },

    paintStars: function(endIndex, stateClass){
      var $polygonLeft;
      var $polygonRight;
      var leftClass;
      var rightClass;

      $.each(this.$stars, function(index, star){
        $polygonLeft = $(star).find('[data-side="left"]');
        $polygonRight = $(star).find('[data-side="right"]');
        leftClass = rightClass = (index <= endIndex) ? stateClass : 'empty';

        // has another half rating, add half star
        leftClass = ( index - endIndex === 0.5 ) ? stateClass : leftClass;

        $polygonLeft.attr('class', 'svg-'  + leftClass + '-' + this._uid);
        $polygonRight.attr('class', 'svg-'  + rightClass + '-' + this._uid);

      }.bind(this));
    },

    renderMarkup: function () {
      var s = this.settings;
      var baseUrl = s.baseUrl ? location.href.split('#')[0] : '';

      // inject an svg manually to have control over attributes
      var star = '<div class="jq-star" style="width:' + s.starSize+ 'px;  height:' + s.starSize + 'px;"><svg version="1.0" class="jq-star-svg" shape-rendering="geometricPrecision" xmlns="http://www.w3.org/2000/svg" ' + this.getSvgDimensions(s.starShape) +  ' stroke-width:' + s.strokeWidth + 'px;" xml:space="preserve"><style type="text/css">.svg-empty-' + this._uid + '{fill:url(' + baseUrl + '#' + this._uid + '_SVGID_1_);}.svg-hovered-' + this._uid + '{fill:url(' + baseUrl + '#' + this._uid + '_SVGID_2_);}.svg-active-' + this._uid + '{fill:url(' + baseUrl + '#' + this._uid + '_SVGID_3_);}.svg-rated-' + this._uid + '{fill:' + s.ratedColor + ';}</style>' +

      this.getLinearGradient(this._uid + '_SVGID_1_', s.emptyColor, s.emptyColor, s.starShape) +
      this.getLinearGradient(this._uid + '_SVGID_2_', s.hoverColor, s.hoverColor, s.starShape) +
      this.getLinearGradient(this._uid + '_SVGID_3_', s.starGradient.start, s.starGradient.end, s.starShape) +
      this.getVectorPath(this._uid, {
        starShape: s.starShape,
        strokeWidth: s.strokeWidth,
        strokeColor: s.strokeColor
      } ) +
      '</svg></div>';

      // inject svg markup
      var starsMarkup = '';
      for( var i = 0; i < s.totalStars; i++){
        starsMarkup += star;
      }
      this.$el.append(starsMarkup);
      this.$stars = this.$el.find('.jq-star');
    },

    getVectorPath: function(id, attrs){
      return (attrs.starShape === 'rounded') ?
        this.getRoundedVectorPath(id, attrs) : this.getSpikeVectorPath(id, attrs);
    },

    getSpikeVectorPath: function(id, attrs){
      return '<polygon data-side="center" class="svg-empty-' + id + '" points="281.1,129.8 364,55.7 255.5,46.8 214,-59 172.5,46.8 64,55.4 146.8,129.7 121.1,241 212.9,181.1 213.9,181 306.5,241 " style="fill: transparent; stroke: ' + attrs.strokeColor + ';" />' +
        '<polygon data-side="left" class="svg-empty-' + id + '" points="281.1,129.8 364,55.7 255.5,46.8 214,-59 172.5,46.8 64,55.4 146.8,129.7 121.1,241 213.9,181.1 213.9,181 306.5,241 " style="stroke-opacity: 0;" />' +
          '<polygon data-side="right" class="svg-empty-' + id + '" points="364,55.7 255.5,46.8 214,-59 213.9,181 306.5,241 281.1,129.8 " style="stroke-opacity: 0;" />';
    },

    getRoundedVectorPath: function(id, attrs){
      var fullPoints = 'M520.9,336.5c-3.8-11.8-14.2-20.5-26.5-22.2l-140.9-20.5l-63-127.7 c-5.5-11.2-16.8-18.2-29.3-18.2c-12.5,0-23.8,7-29.3,18.2l-63,127.7L28,314.2C15.7,316,5.4,324.7,1.6,336.5S1,361.3,9.9,370 l102,99.4l-24,140.3c-2.1,12.3,2.9,24.6,13,32c5.7,4.2,12.4,6.2,19.2,6.2c5.2,0,10.5-1.2,15.2-3.8l126-66.3l126,66.2 c4.8,2.6,10,3.8,15.2,3.8c6.8,0,13.5-2.1,19.2-6.2c10.1-7.3,15.1-19.7,13-32l-24-140.3l102-99.4 C521.6,361.3,524.8,348.3,520.9,336.5z';

      return '<path data-side="center" class="svg-empty-' + id + '" d="' + fullPoints + '" style="stroke: ' + attrs.strokeColor + '; fill: transparent; " /><path data-side="right" class="svg-empty-' + id + '" d="' + fullPoints + '" style="stroke-opacity: 0;" /><path data-side="left" class="svg-empty-' + id + '" d="M121,648c-7.3,0-14.1-2.2-19.8-6.4c-10.4-7.6-15.6-20.3-13.4-33l24-139.9l-101.6-99 c-9.1-8.9-12.4-22.4-8.6-34.5c3.9-12.1,14.6-21.1,27.2-23l140.4-20.4L232,164.6c5.7-11.6,17.3-18.8,30.2-16.8c0.6,0,1,0.4,1,1 v430.1c0,0.4-0.2,0.7-0.5,0.9l-126,66.3C132,646.6,126.6,648,121,648z" style="stroke: ' + attrs.strokeColor + '; stroke-opacity: 0;" />';
    },

    getSvgDimensions: function(starShape){
      return (starShape === 'rounded') ? 'width="550px" height="500.2px" viewBox="0 146.8 550 500.2" style="enable-background:new 0 0 550 500.2;' : 'x="0px" y="0px" width="305px" height="305px" viewBox="60 -62 309 309" style="enable-background:new 64 -59 305 305;';
    },

    getLinearGradient: function(id, startColor, endColor, starShape){
      var height = (starShape === 'rounded') ? 500 : 250;
      return '<linearGradient id="' + id + '" gradientUnits="userSpaceOnUse" x1="0" y1="-50" x2="0" y2="' + height + '"><stop  offset="0" style="stop-color:' + startColor + '"/><stop  offset="1" style="stop-color:' + endColor + '"/> </linearGradient>';
    },

    executeCallback: function(rating, $el){
      var callback = this.settings.callback;
      callback(rating, $el);
    }

  };

  var publicMethods = {

    unload: function() {
      var _name = 'plugin_' + pluginName;
      var $el = $(this);
      var $starSet = $el.data(_name).$stars;
      $starSet.off();
      $el.removeData(_name).remove();
    },

    setRating: function(rating, round) {
      var _name = 'plugin_' + pluginName;
      var $el = $(this);
      var $plugin = $el.data(_name);
      if( rating > $plugin.settings.totalStars || rating < 0 ) { return; }
      if( round ){
        rating = Math.round(rating);
      }
      $plugin.applyRating(rating);
    },

    getRating: function() {
      var _name = 'plugin_' + pluginName;
      var $el = $(this);
      var $starSet = $el.data(_name);
      return $starSet._state.rating;
    },

    resize: function(newSize) {
      var _name = 'plugin_' + pluginName;
      var $el = $(this);
      var $starSet = $el.data(_name);
      var $stars = $starSet.$stars;

      if(newSize <= 1 || newSize > 200) {
        console.log('star size out of bounds');
        return;
      }

      $stars = Array.prototype.slice.call($stars);
      $stars.forEach(function(star){
        $(star).css({
          'width': newSize + 'px',
          'height': newSize + 'px'
        });
      });
    },

    setReadOnly: function(flag) {
      var _name = 'plugin_' + pluginName;
      var $el = $(this);
      var $plugin = $el.data(_name);
      if(flag === true){
        $plugin.$stars.off('mouseover mouseout click');
      } else {
        $plugin.settings.readOnly = false;
        $plugin.addListeners();
      }
    }

  };


  // Avoid Plugin.prototype conflicts
  $.extend(Plugin.prototype, methods);

  $.fn[ pluginName ] = function ( options ) {

    // if options is a public method
    if( !$.isPlainObject(options) ){
      if( publicMethods.hasOwnProperty(options) ){
        return publicMethods[options].apply(this, Array.prototype.slice.call(arguments, 1));
      } else {
        $.error('Method '+ options +' does not exist on ' + pluginName + '.js');
      }
    }

    return this.each(function() {
      // preventing against multiple instantiations
      if ( !$.data( this, 'plugin_' + pluginName ) ) {
        $.data( this, 'plugin_' + pluginName, new Plugin( this, options ) );
      }
    });
  };

})( jQuery, window, document );


$(function() {

  // basic use comes with defaults values
  $(".my-rating").starRating({
    totalStars: 5,
  starSize: 20,
  emptyColor: 'lightgray',
  hoverColor: '#FF0000',
  activeColor: '#FF0000',
  strokeWidth: 0,
  strokeColor: '#FF0000',
  useGradient: false
  });

});


$(".action--open").click(function(){
  $(".overlayBG").addClass("block");
});

$(".action--close").click(function(){
  $(".overlayBG").removeClass("block");
});


function isValidEmail(emailText) {
    var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
    return pattern.test(emailText);
}