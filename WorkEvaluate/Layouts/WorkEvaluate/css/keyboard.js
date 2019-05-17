function VKI_buildKeyboardInputs() {
  var self = this;

  this.VKI_version = "";
  this.VKI_target = this.VKI_visible = "";
  this.VKI_shift = this.VKI_capslock = this.VKI_alternate = this.VKI_dead = false;
  this.VKI_deadkeysOn = false;
  this.VKI_kt = "US";  // Default keyboard layout
  this.VKI_range = false;
  this.VKI_keyCenter = 3;


  /* ***** Create keyboards **************************************** */
  this.VKI_layout = new Object();
  this.VKI_layoutDDK = new Object();

  this.VKI_layout.US = [ // US Standard Keyboard
    [["1"], ["2"], ["3"], ["4"], ["5"]],
    [["6"], ["7"], ["8"], ["9"], ["0"]],
    [["."],["Bksp", "Bksp"],["Enter", "Enter"]],
  ];

 /* this.VKI_layout["US Int'l"] = [ // US International Keyboard
    [["`", "~"], ["1", "!", "\u00a1", "\u00b9"], ["2", "@", "\u00b2"], ["3", "#", "\u00b3"], ["4", "$", "\u00a4", "\u00a3"], ["5", "%", "\u20ac"], ["6", "^", "\u00bc"], ["7", "&", "\u00bd"], ["8", "*", "\u00be"], ["9", "(", "\u2018"], ["0", ")", "\u2019"], ["-", "_", "\u00a5"], ["=", "+", "\u00d7", "\u00f7"], ["Bksp", "Bksp"]],
    [["Tab", "Tab"], ["q", "Q", "\u00e4", "\u00c4"], ["w", "W", "\u00e5", "\u00c5"], ["e", "E", "\u00e9", "\u00c9"], ["r", "R", "\u00ae"], ["t", "T", "\u00fe", "\u00de"], ["y", "Y", "\u00fc", "\u00dc"], ["u", "U", "\u00fa", "\u00da"], ["i", "I", "\u00ed", "\u00cd"], ["o", "O", "\u00f3", "\u00d3"], ["p", "P", "\u00f6", "\u00d6"], ["[", "{", "\u00ab"], ["]", "}", "\u00bb"], ["\\", "|", "\u00ac", "\u00a6"]],
    [["Caps", "Caps"], ["a", "A", "\u00e1", "\u00c1"], ["s", "S", "\u00df", "\u00a7"], ["d", "D", "\u00f0", "\u00d0"], ["f", "F"], ["g", "G"], ["h", "H"], ["j", "J"], ["k", "K"], ["l", "L", "\u00f8", "\u00d8"], [";", ":", "\u00b6", "\u00b0"], ["'", '"', "\u00b4", "\u00a8"], ["Enter", "Enter"]],
    [["Shift", "Shift"], ["z", "Z", "\u00e6", "\u00c6"], ["x", "X"], ["c", "C", "\u00a9", "\u00a2"], ["v", "V"], ["b", "B"], ["n", "N", "\u00f1", "\u00d1"], ["m", "M", "\u00b5"], [",", "<", "\u00e7", "\u00c7"], [".", ">"], ["/", "?", "\u00bf"], ["Shift", "Shift"]],
    [[" ", " ", " ", " "], ["Alt", "Alt"]]
  ];
*/

  /* ***** Define Dead Keys **************************************** */
 
 
 this.VKI_deadkey = new Object();

  // - Lay out each dead key set in one row of sub-arrays.  The rows
  //   below are wrapped so uppercase letters are below their lowercase
  //   equivalents.
  //
  // - The first letter in each sub-array is the letter pressed after
  //   the diacritic.  The second letter is the letter this key-combo
  //   will generate.
  //
  // - Note that if you have created a new keyboard layout and want it
  //   included in the distributed script, PLEASE TELL ME if you have
  //   added additional dead keys to the ones below.



  /* ***** Find tagged input & textarea elements ******************* */
  var inputElems = [
    document.getElementsByTagName('input'),
    document.getElementsByTagName('textarea'),
  ]
  for (var x = 0, inputCount = 0, elem; elem = inputElems[x++];) {
    if (elem) {
      for (var y = 0, keyid = "", ex; ex = elem[y++];) {
        if ((ex.nodeName == "TEXTAREA" || ex.type == "text" || ex.type == "password") && ex.className.indexOf("keyboardInput") > -1) {
          if (!ex.id) {
            do { keyid = 'keyboardInputInitiator' + inputCount++; } while (document.getElementById(keyid));
            ex.id = keyid;
          } else keyid = ex.id;
          var keybut = document.createElement('img');
              keybut.src = "images/keyboard.png";
            
              keybut.alt = "Keyboard interface";
              keybut.className = "keyboardInputInitiator";
              keybut.title = "";
              keybut.onclick = (function(a) { return function() { self.VKI_show(a); }; })(keyid);
          ex.parentNode.insertBefore(keybut, ex.nextSibling);
          if (!window.sidebar && !window.opera) {
            ex.onclick = ex.onkeyup = ex.onselect = function() {
              if (self.VKI_target.createTextRange) self.VKI_range = document.selection.createRange();

            };
          }
        }
      }
    }
  }


  /* ***** Build the keyboard interface **************************** */
  this.VKI_keyboard = document.createElement('table');
  this.VKI_keyboard.id = "keyboardInputMaster";
  this.VKI_keyboard.cellSpacing = this.VKI_keyboard.cellPadding = this.VKI_keyboard.border = "0";

  var layouts = 0;
  for (ktype in this.VKI_layout) if (typeof this.VKI_layout[ktype] == "object") layouts++;

  var thead = document.createElement('thead');
    var tr = document.createElement('tr');
      var th = document.createElement('th');
        if (layouts > 1) {
          var kblist = document.createElement('select');
            for (ktype in this.VKI_layout) {
              if (typeof this.VKI_layout[ktype] == "object") {
                var opt = document.createElement('option');
                    opt.value = ktype;
                    opt.appendChild(document.createTextNode(ktype));
                  kblist.appendChild(opt);
              }
            }
              kblist.value = this.VKI_kt;
              kblist.onchange = function() {
                self.VKI_kt = this.value;
                self.VKI_buildKeys();
                self.VKI_position();
              };
          th.appendChild(kblist);
        }

          var label = document.createElement('label');
            var checkbox = document.createElement('input');
                checkbox.type = "checkbox";
                checkbox.checked = this.VKI_deadkeysOn;
                checkbox.title = "Toggle dead key input";
                checkbox.onclick = function() {
                  self.VKI_deadkeysOn = this.checked;
                  this.nextSibling.nodeValue = " Dead keys: " + ((this.checked) ? "On" : "Off");
                  self.VKI_modify("");
                  return true;
                };
              label.appendChild(checkbox);
              label.appendChild(document.createTextNode(" Dead keys: " + ((checkbox.checked) ? "On" : "Off")))
          th.appendChild(label);
        tr.appendChild(th);

      var td = document.createElement('td');
        var clearer = document.createElement('span');
            clearer.id = "keyboardInputClear";
            clearer.appendChild(document.createTextNode("Clear"));
            clearer.title = "Clear this input";
            clearer.onmousedown = function() { this.className = "pressed"; };
            clearer.onmouseup = function() { this.className = ""; };
            clearer.onclick = function() {
              self.VKI_target.value = "";
              self.VKI_target.focus();
              return false;
            };
          td.appendChild(clearer);

        var closer = document.createElement('span');
            closer.id = "keyboardInputClose";
            closer.appendChild(document.createTextNode('Close'));
            closer.title = "Close this window";
            closer.onmousedown = function() { this.className = "pressed"; };
            closer.onmouseup = function() { this.className = ""; };
            closer.onclick = function() { self.VKI_close(); };
          td.appendChild(closer);

        tr.appendChild(td);
      thead.appendChild(tr);
  this.VKI_keyboard.appendChild(thead);

  var tbody = document.createElement('tbody');
    var tr = document.createElement('tr');
      var td = document.createElement('td');
          td.colSpan = "2";
        var div = document.createElement('div');
            div.id = "keyboardInputLayout";
          td.appendChild(div);
        var div = document.createElement('div');
          var ver = document.createElement('var');
             // ver.appendChild(document.createTextNode("v" + this.VKI_version));
            div.appendChild(ver);
          td.appendChild(div);
        tr.appendChild(td);
      tbody.appendChild(tr);
  this.VKI_keyboard.appendChild(tbody);      



  /* ***** Functions ************************************************ */
  /* ******************************************************************
   * Build or rebuild the keyboard keys
   *
   */
  this.VKI_buildKeys = function() {
    this.VKI_shift = this.VKI_capslock = this.VKI_alternate = this.VKI_dead = false;
    this.VKI_deadkeysOn = (this.VKI_layoutDDK[this.VKI_kt]) ? false : this.VKI_keyboard.getElementsByTagName('label')[0].getElementsByTagName('input')[0].checked;

    var container = this.VKI_keyboard.tBodies[0].getElementsByTagName('div')[0];
    while (container.firstChild) container.removeChild(container.firstChild);

    for (var x = 0, hasDeadKey = false, lyt; lyt = this.VKI_layout[this.VKI_kt][x++];) {
      var table = document.createElement('table');
          table.cellSpacing = table.cellPadding = table.border = "0";
      if (lyt.length <= this.VKI_keyCenter) table.className = "keyboardInputCenter";
        var tbody = document.createElement('tbody');
          var tr = document.createElement('tr');
          for (var y = 0, lkey; lkey = lyt[y++];) {
            if (!this.VKI_layoutDDK[this.VKI_kt] && !hasDeadKey)
              for (var z = 0; z < lkey.length; z++)
                if (this.VKI_deadkey[lkey[z]]) hasDeadKey = true;

            var td = document.createElement('td');
                td.appendChild(document.createTextNode(lkey[0]));

              var alive = false;
              if (this.VKI_deadkeysOn) for (key in this.VKI_deadkey) if (key === lkey[0]) alive = true;
                td.className = (alive) ? "alive" : "";
              if (lyt.length > this.VKI_keyCenter && y == lyt.length)
                td.className += " last";

              if (lkey[0] == " ")
                td.style.paddingLeft = td.style.paddingRight = "50px";
                td.onmouseover = function() { if (this.className != "dead" && this.firstChild.nodeValue != "\xa0") this.className += " hover"; };
                td.onmouseout = function() { if (this.className != "dead") this.className = this.className.replace(/ ?(hover|pressed)/g, ""); };
                td.onmousedown = function() { if (this.className != "dead" && this.firstChild.nodeValue != "\xa0") this.className += " pressed"; };
                td.onmouseup = function() { if (this.className != "dead") this.className = this.className.replace(/ ?pressed/g, ""); };
                td.ondblclick = function() { return false; };

              switch (lkey[1]) {
                case "Caps":
                case "Shift":
                case "Alt":
                case "AltGr":
                  td.onclick = (function(type) { return function() { self.VKI_modify(type); return false; }})(lkey[1]);
                  break;
                case "Tab":
                  td.onclick = function() { self.VKI_insert("\t"); return false; };
                  break;
                case "Bksp":
                  td.onclick = function() {
                    self.VKI_target.focus();
                    if (self.VKI_target.setSelectionRange) {
                      var srt = self.VKI_target.selectionStart;
                      var len = self.VKI_target.selectionEnd;
                      if (srt < len) srt++;
                      self.VKI_target.value = self.VKI_target.value.substr(0, srt - 1) + self.VKI_target.value.substr(len);
                      self.VKI_target.setSelectionRange(srt - 1, srt - 1);
                    } else if (self.VKI_target.createTextRange) {
                      try { self.VKI_range.select(); } catch(e) {}
                      self.VKI_range = document.selection.createRange();
                      if (!self.VKI_range.text.length) self.VKI_range.moveStart('character', -1);
                      self.VKI_range.text = "";
                    } else self.VKI_target.value = self.VKI_target.value.substr(0, self.VKI_target.value.length - 1);
                    if (self.VKI_shift) self.VKI_modify("Shift");
                    if (self.VKI_alternate) self.VKI_modify("AltGr");
                    return true;
                  };
                  break;
                case "Enter":
                  td.onclick = function() {
                    if (self.VKI_target.nodeName == "TEXTAREA") { self.VKI_insert("\n"); } else self.VKI_close();
                    return true;
                  };
                  break;
                default:
                  td.onclick = function() {
                    if (self.VKI_deadkeysOn && self.VKI_dead) {
                      if (self.VKI_dead != this.firstChild.nodeValue) {
                        for (key in self.VKI_deadkey) {
                          if (key == self.VKI_dead) {
                            if (this.firstChild.nodeValue != " ") {
                              for (var z = 0, rezzed = false, dk; dk = self.VKI_deadkey[key][z++];) {
                                if (dk[0] == this.firstChild.nodeValue) {
                                  self.VKI_insert(dk[1]);
                                  rezzed = true;
                                  break;
                                }
                              }
                            } else {
                              self.VKI_insert(self.VKI_dead);
                              rezzed = true;
                            }
                            break;
                          }
                        }
                      } else rezzed = true;
                    }
                    self.VKI_dead = false;

                    if (!rezzed && this.firstChild.nodeValue != "\xa0") {
                      if (self.VKI_deadkeysOn) {
                        for (key in self.VKI_deadkey) {
                          if (key == this.firstChild.nodeValue) {
                            self.VKI_dead = key;
                            this.className = "dead";
                            if (self.VKI_shift) self.VKI_modify("Shift");
                            if (self.VKI_alternate) self.VKI_modify("AltGr");
                            break;
                          }
                        }
                        if (!self.VKI_dead) self.VKI_insert(this.firstChild.nodeValue);
                      } else self.VKI_insert(this.firstChild.nodeValue);
                    }

                    self.VKI_modify("");
                    return false;
                  };

              }
              tr.appendChild(td);
            tbody.appendChild(tr);
          table.appendChild(tbody);

          for (var z = lkey.length; z < 4; z++) lkey[z] = "\xa0";
      }
      container.appendChild(table);
    }
    this.VKI_keyboard.getElementsByTagName('label')[0].style.display = (hasDeadKey) ? "inline" : "none";
  };

  this.VKI_buildKeys();
  VKI_disableSelection(this.VKI_keyboard);


  /* ******************************************************************
   * Controls modifier keys
   *
   */
  this.VKI_modify = function(type) {
    switch (type) {
      case "Alt":
      case "AltGr": this.VKI_alternate = !this.VKI_alternate; break;
      case "Caps": this.VKI_capslock = !this.VKI_capslock; break;
      case "Shift": this.VKI_shift = !this.VKI_shift; break;
    }
    var vchar = 0;
    if (!this.VKI_shift != !this.VKI_capslock) vchar += 1;

    var tables = this.VKI_keyboard.getElementsByTagName('table');
    for (var x = 0; x < tables.length; x++) {
      var tds = tables[x].getElementsByTagName('td');
      for (var y = 0; y < tds.length; y++) {
        var dead = alive = target = false;
        var lkey = this.VKI_layout[this.VKI_kt][x][y];

        switch (lkey[1]) {
          case "Alt":
          case "AltGr":
            if (this.VKI_alternate) dead = true;
            break;
          case "Shift":
            if (this.VKI_shift) dead = true;
            break;
          case "Caps":
            if (this.VKI_capslock) dead = true;
            break;
          case "Tab": case "Enter": case "Bksp": break;
          default:
            if (type) tds[y].firstChild.nodeValue = lkey[vchar + ((this.VKI_alternate && lkey.length == 4) ? 2 : 0)];
            if (this.VKI_deadkeysOn) {
              var tempchar = tds[y].firstChild.nodeValue;
              if (this.VKI_dead) {
                  if ( tempchar == this.VKI_dead ) dead = true;
                for (var z = 0; z < this.VKI_deadkey[this.VKI_dead].length; z++)
                    if ( tempchar == this.VKI_deadkey[this.VKI_dead][z][0] ) { target = true; break; }
              }
              for ( key in this.VKI_deadkey ) if ( key === tempchar ) { alive = true; break; }
            }
        }

        tds[y].className = (dead) ? "dead" : ((target) ? "target" : ((alive) ? "alive" : ""));
        if (y == tds.length - 1 && tds.length > this.VKI_keyCenter) tds[y].className += " last";
      }
    }
    this.VKI_target.focus();
  };


  /* ******************************************************************
   * Insert text at the cursor
   *
   */
  this.VKI_insert = function(text) {
    this.VKI_target.focus();
    if (this.VKI_target.setSelectionRange) {
      var srt = this.VKI_target.selectionStart;
      var len = this.VKI_target.selectionEnd;
      this.VKI_target.value = this.VKI_target.value.substr(0, srt) + text + this.VKI_target.value.substr(len);
      if (text == "\n" && window.opera) srt++;
      this.VKI_target.setSelectionRange(srt + text.length, srt + text.length);
    } else if (this.VKI_target.createTextRange) {
      try { this.VKI_range.select(); } catch(e) {}
      this.VKI_range = document.selection.createRange();
      this.VKI_range.text = text;
      this.VKI_range.collapse(true);
      this.VKI_range.select();
    } else this.VKI_target.value += text;
    if (this.VKI_shift) this.VKI_modify("Shift");
    if (this.VKI_alternate) this.VKI_modify("AltGr");
    this.VKI_target.focus();
  };


  /* ******************************************************************
   * Show the keyboard interface
   *
   */
  this.VKI_show = function(id) {
    if (this.VKI_target = document.getElementById(id)) {
      if (this.VKI_visible != id) {
        this.VKI_range = "";
        try { this.VKI_keyboard.parentNode.removeChild(this.VKI_keyboard); } catch (e) {}

        var elem = this.VKI_target;
        this.VKI_target.keyboardPosition = "absolute";
        do {
          if (VKI_getStyle(elem, "position") == "fixed") {
            this.VKI_target.keyboardPosition = "fixed";
            break;
          }
        } while (elem = elem.offsetParent);

        this.VKI_keyboard.style.top = this.VKI_keyboard.style.right = this.VKI_keyboard.style.bottom = this.VKI_keyboard.style.left = "auto";
        this.VKI_keyboard.style.position = this.VKI_target.keyboardPosition;
        document.body.appendChild(this.VKI_keyboard);

        this.VKI_visible = this.VKI_target.id;
        this.VKI_position();
        this.VKI_target.focus();
      } else this.VKI_close();
    }
  };


  /* ******************************************************************
   * Position the keyboard
   *
   */
  this.VKI_position = function() {
    if (self.VKI_visible != "") {
      var inputElemPos = VKI_findPos(self.VKI_target);
      self.VKI_keyboard.style.top = inputElemPos[1] - ((self.VKI_target.keyboardPosition == "fixed") ? document.body.scrollTop : 0) + self.VKI_target.offsetHeight + 3 + "px";
      self.VKI_keyboard.style.left = Math.min(VKI_innerDimensions()[0] - self.VKI_keyboard.offsetWidth - 15, inputElemPos[0]) + "px";
    }
  };


  if (window.addEventListener) {
    window.addEventListener('resize', this.VKI_position, false); 
  } else if (window.attachEvent)
    window.attachEvent('onresize', this.VKI_position);


  /* ******************************************************************
   * Close the keyboard interface
   *
   */
  this.VKI_close = function() {
    try { this.VKI_keyboard.parentNode.removeChild(this.VKI_keyboard); } catch (e) {}
    this.VKI_visible = "";
    this.VKI_target.focus();
    this.VKI_target = "";
  };
}


/* ***** Attach this script to the onload event ******************** */
if (window.addEventListener) {
  window.addEventListener('load', VKI_buildKeyboardInputs, false); 
} else if (window.attachEvent)
  window.attachEvent('onload', VKI_buildKeyboardInputs);


function VKI_findPos(obj) {
  var curleft = curtop = 0;
  do {
    curleft += obj.offsetLeft;
    curtop += obj.offsetTop;
  } while (obj = obj.offsetParent);    
  return [curleft, curtop];
}

function VKI_innerDimensions() {
  if (self.innerHeight) {
    return [self.innerWidth, self.innerHeight];
  } else if (document.documentElement && document.documentElement.clientHeight) {
    return [document.documentElement.clientWidth, document.documentElement.clientHeight];
  } else if (document.body)
    return [document.body.clientWidth, document.body.clientHeight];
  return [0, 0];
}

function VKI_getStyle(obj, styleProp) {
  if (obj.currentStyle) {
    var y = obj.currentStyle[styleProp];
  } else if (window.getComputedStyle)
    var y = window.getComputedStyle(obj, null)[styleProp];
  return y;
}

function VKI_disableSelection(elem) {
  elem.onselectstart = function() { return false; };
  elem.unselectable = "on";
  elem.style.MozUserSelect = "none";
  elem.style.cursor = "default";
  if (window.opera) elem.onmousedown = function() { return false; };
}