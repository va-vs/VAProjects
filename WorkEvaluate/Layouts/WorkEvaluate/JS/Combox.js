Function.prototype.BindForEvent = function ()
{
    var __m = this, object = arguments[0], args = new Array();
    for ( var i = 1; i < arguments.length; i++ )
    {
        args.push( arguments[i] );
    }

    return function ( event )
    {
        return __m.apply( object, [( event || window.event )].concat( args ) );
    }
};

var Class = {
    create: function ()
    {
        return function ()
        {
            this.init.apply( this, arguments );
        }
    }
}

Object.extend = function ( destination, source )
{
    for ( var property in source )
    {
        destination[property] = source[property];
    }
    return destination;
}

function Offset( e )
{
    var t = e.offsetTop;
    var l = e.offsetLeft;
    var w = e.offsetWidth;
    var h = e.offsetHeight - 2;
    var f = '';

    while ( e = e.offsetParent )
    {
        f += ( e.id || e.className || e.tagName ) + e.offsetTop + "---";
        t += e.offsetTop;
        l += e.offsetLeft;
    }
    //alert(f);
    return { top: t, left: l, width: w, height: h }
}

function $()
{
    var target = arguments[0];
    if ( typeof target == "object" )
    {
        return target;
    }
    return document.getElementById( target );
}
var Comboxs = [];
var Combox = Class.create();
Combox.prototype = {
    DrpID: 'ddlCourses',
    offset: null,
    DrpList: null,
    SetHeight: 22, //高
    box: null,
    input: null,
    Btn: null,
    Optionsbox: null,
    seledOption: null,
    seledText: '',
    seledValue: '',
    bJoin: true,//是否联动
    JoinID: 'ddlPeriods', //联动对象ID
    w: 0,
    inputW: 0,
    s: 1, // 0  编辑页使用  1列表页显示
    NumOption: 0,
    init: function ( objID, w, inputW, s )
    {
        this.s = s || 0;
        this.DrpID = objID
        this.DrpList = $( objID );
        if ( !this.DrpList ) return;
        this.w = w;
        this.inputW = inputW;
        //this.offset = Offset(this.DrpList);
        Comboxs.push( [this.DrpID, this] );
        if ( this.DrpList.getAttribute( "joinDrp" ) != null && this.DrpList.getAttribute( "joinDrp" ) != "" )
        {
            this.bJoin = true;
            this.JoinID = this.DrpList.getAttribute( "joinDrp" );
        }
        this.CreateBox();
    },
    CreateBox: function ()
    {
        var Div = document.createElement( "DIV" );
        if ( this.s == 0 )
        {
            Div.className = "DrpBox_normal";
        }
        else
        {
            Div.className = "sDrpBox_normal";
        }
        Div.setAttribute( "id", this.DrpList.id + "_" + "BOX" );
        Div.style.width = this.w + "px";
        if ( this.DrpList.style.display == 'none' )
        {
            Div.style.display = 'none';
        }

        this.DrpList.style.display = 'none';
        this.box = Div;

        var parentobj = this.DrpList.parentNode;
        if ( !parentobj ) return;
        parentobj.insertBefore( Div, this.DrpList );
        this.CreateInput();
        this.CreateButton();
        this.CreateOptions();
    },
    CreateInput: function ()
    {
        var Div = document.createElement( "DIV" );
        //Div.style.position ="absolute";
        Div.setAttribute( "id", this.DrpList.id + "_" + "INPUT" );
        Div.className = "DrpInput_normal";
        Div.style.width = this.inputW + "px";
        Div.style.overflow = 'hidden';
        this.box.appendChild( Div );
        this.input = Div;
        this.input.onmouseout = this.HidOptionBoxOutInput.BindForEvent( this.input, this );
    },
    CreateButton: function ()
    {
        var Div = document.createElement( "DIV" );
        Div.className = "DrpBtn";
        Div.setAttribute( "id", this.DrpList.id + "_" + "Btn" );
        this.Btn = Div;
        this.box.appendChild( Div );
        this.Btn.onmouseout = this.HidOptionBoxOutInput.BindForEvent( this.Btn, this );
    },
    CreateOptions: function ()
    {
        this.Optionsbox = null;
        var Div = document.createElement( "DIV" );
        Div.style.display = 'none';
        Div.style.width = this.w + "px";

        Div.className = "DrpOptionsBox";
        Div.setAttribute( "id", this.DrpList.id + "_" + "Optionsbox" );

        if ( !browser.msie )
        {
            var f = Offset( this.box );
            Div.style.top = f.top + document.documentElement.scrollTop + this.SetHeight + "px";
            Div.style.left = f.left + "px";
        }
        var parentobj = this.DrpList.parentNode;
        if ( !parentobj ) return;
        parentobj.insertBefore( Div, this.DrpList );
        this.Optionsbox = Div;
        var Nums = this.DrpList.options.length;
        this.NumOption = Nums;

        for ( var i = 0; i < Nums; i++ )
        {
            var DivOp = document.createElement( "DIV" );
            DivOp.innerHTML = this.DrpList.options[i].innerHTML;
            DivOp.setAttribute( "value", this.DrpList.options[i].value );
            DivOp.setAttribute( "index", i );
            DivOp.style.cursor = "default";
            if ( this.DrpList.options[i].selected )
            {
                DivOp.setAttribute( "selected", "true" );
                DivOp.className = "DrpOptionsSeled";
                this.seledOption = DivOp;
                this.seledText = DivOp.innerHTML;
                this.seledValue = DivOp.getAttribute( "value" );
                this.input.innerHTML = this.seledText;
            }
            else
            {
                DivOp.setAttribute( "selected", "false" );
                DivOp.className = "DrpOptions";
            }
            Div.appendChild( DivOp );
            DivOp.onmouseover = this.Option_onMouseover.BindForEvent( DivOp, DivOp, this );
            DivOp.onmouseout = this.Option_onMouseout.BindForEvent( DivOp, DivOp, this );
            DivOp.onclick = this.Option_onClick.BindForEvent( DivOp, DivOp, this );
        }
        if ( Nums > 6 )
        {
            Div.style.overflow = "auto";
            Div.style.height = this.SetHeight * 6 + "px";
        }
        else
        {
            Div.style.height = this.SetHeight * Nums + "px";
        }
        this.Optionsbox.onmouseout = this.HidOptionBoxEnt.BindForEvent( this.box, this );
        this.input.onclick = this.ShowOptionBox.BindForEvent( this.input, this );
        this.Btn.onclick = this.ShowOptionBox.BindForEvent( this.input, this );
    },
    Option_onMouseover: function ()
    {
        var obj = arguments[1];
        if ( obj.className == "DrpOptionsSeled" )
        {
            return;
        }
        else
        {
            obj.className = "DrpOptionsOver";
        }
    },
    Option_onMouseout: function ()
    {
        var obj = arguments[1];
        if ( obj.className == "DrpOptionsSeled" )
        {
            return;
        }
        else
        {
            obj.className = "DrpOptions";
        }
    },
    CloseCombox: function ( ojbID )
    {
        for ( var i = 0; i < Comboxs.length; i++ )
        {
            if ( Comboxs[i][0] == ojbID )
            {
                continue;
            }
            if ( Comboxs[i][1].Optionsbox.style.display == '' )
            {
                Comboxs[i][1].Optionsbox.style.display = 'none';
            }
        }
    },
    Option_onClick: function ()
    {
        var opt = arguments[1];
        var obj = arguments[2];

        var sel = opt.getAttribute( "selected" );
        if ( sel == 'false' )
        {
            obj.seledOption.setAttribute( "selected", "false" );
            obj.seledOption.className = "DrpOptions";
            opt.setAttribute( "selected", "true" );
            opt.className = "DrpOptionsSeled";
            obj.seledOption = opt;
            obj.seledText = opt.innerHTML;
            obj.seledValue = opt.getAttribute( "value" );
            obj.input.innerHTML = obj.seledText;
        }
        var i = parseInt( opt.getAttribute( "index" ) );
        obj.DrpList.options[i].selected = true;
        obj.HidOptionBox( obj );
        if ( sel == 'false' )
        {
            if ( obj.DrpList.onchange )
            {
                obj.DrpList.onchange();
            }
            if ( obj.bJoin )
            {
                for ( var i = 0; i < Comboxs.length; i++ )
                {
                    if ( obj.JoinID == Comboxs[i][0] )
                    {
                        Comboxs[i][1].CreateOptions();
                        break;
                    }
                }
            }
        }
        if ( obj.DrpList.onblur )
        {
            obj.DrpList.onblur( obj.DrpList );
        }
    },
    ShowOptionBox: function ()
    {
        var obj = arguments[1];
        obj.CloseCombox( obj.DrpID ); //关闭打开得
        if ( obj.Optionsbox.style.display == '' )
        {
            obj.HidOptionBox( obj );
            if ( obj.DrpList.onblur )
            {
                obj.DrpList.onblur( obj.DrpList );
            }
            return;
        }
        obj.Optionsbox.style.display = '';
        if ( obj.DrpList.onfocus )
        {
            obj.DrpList.onfocus( obj.DrpList );
        }
        if ( this.s == 0 )
        {
            obj.box.className = "DrpBox_focus";
        }
    },
    HidOptionBox: function ()
    {
        var obj = arguments[0];
        obj.Optionsbox.style.display = 'none';
        if ( this.s == 0 )
        {
            obj.box.className = "DrpBox_normal";
        }
    },
    IsOptionsboxUp: function ()
    {
        alert( 1 );
    },
    HidOptionBoxEnt: function ()
    {
        var obj = arguments[1];
        var evt = arguments[0];

        var boxOffset = Offset( obj.box );
        var left = boxOffset.left;
        var tops = boxOffset.top;
        var w = boxOffset.width;
        var h = obj.SetHeight * obj.NumOption;
        var scrlTop = document.documentElement.scrollTop;

        if ( obj.NumOption > 6 )
        {
            h = obj.SetHeight * 6;
        }
        if ( evt.clientX > left && evt.clientX < ( left + w ) && evt.clientY + scrlTop > tops + 2 && evt.clientY + scrlTop < ( tops + h + obj.SetHeight ) )
        {
            return;
        }
        obj.Optionsbox.style.display = 'none';
        if ( obj.s == 0 )
        {
            obj.box.className = "DrpBox_normal";
        }
        if ( obj.DrpList.onblur )
        {
            obj.DrpList.onblur( obj.DrpList );
        }
    },
    HidOptionBoxOutInput: function ()
    {
        var obj = arguments[1];
        var evt = arguments[0];

        var boxOffset = Offset( obj.box );
        var left = boxOffset.left;
        var tops = boxOffset.top;
        var w = boxOffset.width;
        var h = boxOffset.height;
        var scrlTop = document.documentElement.scrollTop;
        if ( evt.clientX > left && evt.clientX < ( left + w ) && evt.clientY + scrlTop > tops + 5 )
        {
            return;
        }
        obj.Optionsbox.style.display = 'none';
        if ( obj.s == 0 )
        {
            obj.box.className = "DrpBox_normal";
        }
        if ( obj.DrpList.onblur )
        {
            obj.DrpList.onblur( obj.DrpList );
        }
    }
};
var userAgent = navigator.userAgent.toLowerCase();
var browser = {
    opera: /opera/.test( userAgent ),
    msie: /msie/.test( userAgent ) && !/opera/.test( userAgent ),
    mozilla: /mozilla/.test( userAgent ) && !/(compatible|webkit)/.test( userAgent )
};
function getEvent()
{
    return window.event ? window.event : getEvent.caller.arguments[0];
};
function ResizeWin()
{
    if ( !browser.msie )
    {
        for ( var i = 0; i < Comboxs.length; i++ )
        {
            Comboxs[i][1].Optionsbox.style.top = Offset( Comboxs[i][1].box ).top + 22 + "px";
            Comboxs[i][1].Optionsbox.style.left = Offset( Comboxs[i][1].box ).left + "px";
        }
    }
}
if ( !window.Event )
{
    var Event = new Object();
}
Object.extend( Event, {
    add: function ( element, name, observe, userCapture )
    {
        if ( name == 'keypress' &&
        ( navigator.appVersion.match( /Konqueror|Safari|KHTML/ )
        || element.attachEvent ) )
            name = 'keydown';
        userCapture = userCapture || false;
        if ( element.addEventListener )
        {
            element.addEventListener( name, observe, userCapture );
        }
        else if ( element.attachEvent )
        {
            element.attachEvent( 'on' + name, observe );
        }
    }

} )
Event.add( window, "resize", ResizeWin, true )
