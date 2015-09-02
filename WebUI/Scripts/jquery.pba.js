(function($) {

"use strict";

var util = {
	intersects: function(rect, point) {
		return point.x >= rect.left &&
			point.x <= rect.right &&
			point.y >= rect.top &&
			point.y <= rect.bottom;
	},
	notIntersects: function(rect, point) {
		return point.x < rect.left ||
			point.x > rect.right ||
			point.y < rect.top ||
			point.y > rect.bottom;
	},
	uid: (function() {
		var id = 1;
		return function() {
			return id++;
		};
	}())
};

// ######################
// CLASS: OBSERVABLE AREA
// ######################
var ObservableArea = (function($) {
	var pool = [];
	var ObservableArea = function(el) {
		pool.push(this);

		this._el = el;
		this._onChange = [];
		this._mouseInBounds = false;
	};

	ObservableArea.prototype = {
		// #######
		// PRIVATE
		// #######
		_onMouseMove: function(e) {
			var self = this;
			var before = this._mouseInBounds;
			var now = util.intersects(this.bounds, {
				x: e.pageX, y: e.pageY
			});

			this._mouseInBounds = now;

			if (before !== now) {
				this._onChange.forEach(function(fn) {
					fn(self, now);
				});
			}
		},

		// #######
		// GETTERS
		// #######
		get el() {
			return this._el;
		},
		get bounds() {
			var offset = this._el.offset();
			var width = this._el.outerWidth();
			var height = this._el.outerHeight();
			return {
				top: offset.top,
				left: offset.left,
				right: offset.left + width,
				bottom: offset.top + height,
				width: width,
				height: height
			};
		},
		get mouseInBounds() {
			return this._mouseInBounds;
		},

		// #######
		// METHODS
		// #######
		onChange: function(fn) {
			if (typeof fn === "function") {
				this._onChange.push(fn);
			}
		}
	};

	$(function() {
		$(document.body).mousemove(function(e) {
			pool.forEach(function(obs) {
				if (obs) obs._onMouseMove(e);
			});
		});
	});

	return ObservableArea;
}($));

// ####################
// PLUGIN: PBA DROPDOWN
// ####################
/**
 * @method pbaDropdown
 * @param {string} opts.selector
 * @param {boolean} (opts.absolute)
 * @param {number} (opts.delay)
 * @param {number||boolean} (opts.maxWidth)
 * @param {string} (opts.trigger)
 * @param {number} (opts.zIndex)
 */
$.fn.pbaDropdown = (function($) {
	var pool = [];
	pool.hideAll = function() {
		this.forEach(function(pbaDropdown) {
			if (pbaDropdown) pbaDropdown.hide();
		});
	};
	pool.hideExcept = function(exceptThat) {
		this.forEach(function(pbaDropdown) {
			if (pbaDropdown && pbaDropdown !== exceptThat) {
				pbaDropdown.hide();
			}
		});
	};

	var PbaDropdown = function(toggle, content, opts) {
		pool.push(this);

		this._contentArea = new ObservableArea(content);
		this._displayed = false;
		this._toggle = toggle;
		this._opts = {
			absolute: opts.absolute === true || false,
			delay: opts.delay || 250,
			maxWidth: opts.maxWidth || false,
			trigger: opts.trigger || "click",
			zIndex: opts.zIndex || 1000
		};

		this._initStyles();
		this._initEvents();
	};

	PbaDropdown.prototype = {
		// #######
		// PRIVATE
		// #######
		_initStyles: function() {
			this._contentArea.el.css({
				position: this._opts.position === "absolute" ? this._opts.position : "fixed",
				maxWidth: this._opts.maxWidth ? this._opts.maxWidth + "px" : "none",
				zIndex: this._opts.zIndex || 1000,
				display: "none"
			});
		},
		_initEvents: function() {
			if (this._opts.trigger === "hover" ||
				this._opts.trigger === "both") {
				this._initHoverEvents();
			}

			if (this._opts.trigger === "click" ||
				this._opts.trigger === "both") {
				this._initClickEvents();
			}
		},
		_initClickEvents: function() {
			var self = this;
			this._toggle.click(function(e) {
				e.preventDefault();
				e.stopPropagation();

				self.toggle();

				if (!self._displayed) {
					return pool.hideAll();
				}

				pool.hideExcept(self);
			});
		},
		_initHoverEvents: function() {
			var self = this;
			this._mouseOnToggle = false;

			this._toggle.hover(function() {
				self._lastHoverTimestamp = Date.now();
				self._mouseOnToggle = true;
				self.show();
			}, function() {
				self._mouseOnToggle = false;
				self._deferredHandleMousePosition(self._opts.delay);
			});

			this._contentArea.onChange(function(area, hovered) {
				if (!self._displayed) return;
				if (hovered) {
					self._lastHoverTimestamp = Date.now();
					return;
				}

				self._deferredHandleMousePosition(self._opts.delay);
			});
		},
		_deferredHandleMousePosition: function(delay) {
			clearTimeout(this._timeoutCode);
			
			var self = this;
			this._timeoutCode = setTimeout(function onTimeout() {

				if (Date.now() - self._lastHoverTimestamp < delay) {
					return self._deferredHandleMousePosition(delay);
				}

				var mouseIsOutside = !self._mouseOnToggle &&
					!self._contentArea.mouseInBounds;

				if (mouseIsOutside) self.hide();
			}, delay);
		},
		_updatePosition: function() {
			var toggleOffset = this._toggle.offset();
			var contentTop = toggleOffset.top + this._toggle.outerHeight();

			this._contentArea.el.css({
				top: contentTop + "px",
				left: toggleOffset.left + "px"
			});
		},

		// #########
		// ANIMATION
		// #########
		_show: function() {
			this._contentArea.el.fadeIn();
		},
		_hide: function() {
			this._contentArea.el.fadeOut();
		},

		// #######
		// METHODS
		// #######
		show: function() {
			if (this._displayed) return;

			this._updatePosition();
			this._show();
			this._displayed = true;

			if (this._opts.trigger === "hover") {
				return;
			}

			var self = this;
			$(document.body).one("click", function onOutsideClick(e) {
				var $target = $(e.target);
				var clickOnContent = $target.is(self._contentArea.el) ||
					$target.closest(self._contentArea.el).length > 0;

				if (!clickOnContent) {
					self.hide();
					return;
				}

				$(document.body).one("click", onOutsideClick);
			});
		},
		hide: function() {
			if (!this._displayed) return;
			this._hide();
			this._displayed = false;
		},
		toggle: function(enable) {
			enable = typeof enable === "undefined" ?
				!this._displayed : enable;

			this[enable ? "show" : "hide"]();
		}
	};

	return function(opts) {
		if (!opts || !opts.selector) {
			window.pbaAPI.error("pbaDropdown requires opts.selector");
			return this;
		}

		var dropdownContent = $(opts.selector);
		if (dropdownContent.length === 0) {
			console.error("pbaDropdown: element with selector " +
				opts.selector + " is not found.");
			return this;
		}

		var pbaDropdown = new PbaDropdown(this, dropdownContent, opts || {});
		this.data("pbaDropdown", pbaDropdown);
	}
}($));

}(window.jQuery));