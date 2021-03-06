function hifi() {
    $("#fctTitle").html("HiFi");
    var body = initBodyPage('hifi');
        
    var list = initPageList('hifi');
    var tracks = Array();
    var currSong;
    var thkCurr;
    var thkPrev;
    var thkNext;
    var playing = false;

    
    body.append("<div id='playerscontainer' class='playerscontainer'><div id='trackViewer'><div id='prevTrack' class='player prev'></div><div id='curTrack' class='player curr'></div><div id='nextTrack' class='player next'></div></div><div class='wrapper'></div></div>");
    
    body.append("<div class='row' id='rowTimeControl'><div class='currTime col-xs-1'>--:--</div>"
            + "<div class='sliderContainer col-xs-10'><div id='slider' class=''></div></div>"
            + "<div class='totalTime col-xs-1'>--:--</div></div>");
    
    body.append("<div id='playerControls' class='btn-group'>"
                + "<button id='btnPrev' class='btn btn-default' data-loading-text=\"<span class='glyphicon glyphicon-step-backward'></span>\">"
                    + "<span class='glyphicon glyphicon-step-backward'></span>"
                + "</button>"
                + "<button id='btnPlay' class='btn btn-default' data-loading-text=\"<span class='glyphicon glyphicon-play'></span>\" data-pause-text=\"<span class='glyphicon glyphicon-pause'></span>\">"
                    + "<span class='glyphicon glyphicon-play'></span>"
                + "</button>"
                + "<button id='btnNext' class='btn btn-default' data-loading-text=\"<span class='glyphicon glyphicon-step-forward'></span>\">"
                    + "<span class='glyphicon glyphicon-step-forward'></span>"
                + "</button>"
              + "</div>"
              + "<button id='btnRandom' class='btn btn-default pull-right' >"
                + "<span class='glyphicon glyphicon-random'></span>"
              + "</button>"
            );
      
    //on init, disable buttons
    $("#btnPrev").button('loading');
    $("#btnPlay").button('loading');
    $("#btnNext").button('loading');
    $( "#slider" ).slider({ disabled: true });
    
    $("#btnRandom").click(function(){
        console.info( "randomizing");
        //shuffle
        tracks = shuffle( tracks );
        //attach handler to play on resolved
        $("body").on("hifi.player.playble", function(){
            $("body").off("hifi.player.playble");
            $("#btnPlay").trigger("player.playrequested");
        });
        playing = false;
        //re-init
        initFromTrackList( 0 );
        
    });
    
    $("#btnPlay").click(function(){
        if( playing ) {
            playing = false;
            thkCurr.pause();
            $("#btnPlay").button('reset');
            console.log("pausing");
        }
        else {
            playing = true;
             thkCurr.play();
             $("#btnPlay").button('pause');
             console.log("playing");
        }
    });
    
    $("#btnNext").on("player.nextrequested", function(){
        if( $("#btnNext").hasClass("disabled") ){
            $("#btnNext").on("hifi.player.next.playble", function(){
                $("#btnNext").off("hifi.player.next.playble");
                $("#btnNext").trigger("click");
            });
        }
        else {
            $("#btnNext").trigger("click");
        }
    });
    
    $("#btnPrev").on("player.nextrequested", function(){
        if( $("#btnPrev").hasClass("disabled") ){
            $("#btnPrev").on("hifi.player.previous.playble", function(){
                $("#btnPrev").off("hifi.player.previous.playble");
                $("#btnPrev").trigger("click");
            });
        }
        else {
            $("#btnPrev").trigger("click");
        }
    });
    
    $("#btnPlay").on("player.playrequested", function(){
        if( !playing )
            $("#btnPlay").trigger("click");
    });
    
    $("#btnPlay").on("player.pauserequested", function(){
        if( playing )
            $("#btnPlay").trigger("click");
    });
    
    $("#btnPlay").on("player.playsongrequested", function( e, id ){
        $("body").on("hifi.player.playble", function(){
            $("body").off("hifi.player.playble");
            $("#btnPlay").trigger("player.playrequested");
        });
        playing = false;
        //re-init
        initFromTrackList( findSongById( id ) );
    });
    
    $("#btnPrev").click(function(){
        $("#btnPrev").button('loading');
        $(".next").remove();
        $(".curr").removeClass('curr').addClass('next');
        $(".prev").removeClass('prev').addClass('curr');
        $("#trackViewer").prepend("<div class='player prev'></div>");
        //next Track
        currSong = getPrevious( currSong );
        thkCurr.seek(0);
        if( playing )
            thkCurr.pause(); //pause() play if track is in pause...
        thkNext = thkCurr;
        thkCurr = thkPrev;
        thkPrev = getTHKW( getPrevious( currSong ) );
        renderTrack( thkPrev, $(".prev") );
        $(".wrapper").attr("style","z-index:1000");
        thkCurr.seek(0);
        thkCurr.play();
        playing = true;
        $("#btnPlay").button('pause');
    });
    
    $("#btnNext").click(function(){
        $("#btnNext").button('loading');
        $(".prev").remove();
        $(".curr").removeClass('curr').addClass('prev');
        $(".next").removeClass('next').addClass('curr');
        $("#trackViewer").append("<div class='player next'></div>");
        //prev Track
        currSong = getNext( currSong );
        thkCurr.seek(0);
        if( playing )
            thkCurr.pause(); //pause() play if track is in pause...
        thkPrev = thkCurr;
        thkCurr = thkNext;
        thkNext = getTHKW( getNext( currSong ) );
        renderTrack( thkNext, $(".next") );
        $(".wrapper").attr("style","z-index:1000");
        thkCurr.seek(0);
        thkCurr.play();
        playing = true;
        $("#btnPlay").button('pause');
    });
    
//recuperation des tracks
    $.getJSON( getControllerActionUrl("hifi", "lister"), function( data ){
        if( data.code < 300 ) {
            tracks = data.songs;
            
            $.each( data.songs, function( key, val ) {
                
                console.log( val.id + ' : ' + val.artist + ' - ' + val.title + ' - ' + findSong( val ) + ' - ' + getPrevious( val ).title + ' - ' + getNext( val ).title);
            });
            //generate players
            initFromTrackList( 0 );
            
            //we are ready to play, say it to the world !
            $("body").trigger("hifi.page.ready");
            
        }
        else {
            notify("danger", data.message, "Error", 5000);
        }
    });
    
    function initFromTrackList( trackId ) {
        $("#btnPrev").button('loading');
        $("#btnPlay").button('loading');
        $("#btnNext").button('loading');
        $( "#slider" ).slider({ disabled: true });
        
        currSong = tracks[ trackId ];
        thkCurr = getTHKW( currSong );
        renderTrack( thkCurr, $(".curr") );

        thkPrev = getTHKW( getPrevious( currSong ) );
        renderTrack( thkPrev, $(".prev") );

        thkNext = getTHKW( getNext( currSong ) );
        renderTrack( thkNext, $(".next") );
        console.info( "player initialized");
    }
    
    function getPrevious( song ){
        var index = findSong( song );
        if( index === 0 ) {
            return tracks[tracks.length - 1];
        }
        else {
            return tracks[ index - 1 ];
        }
    }
    
    function getNext( song ){
        var index = findSong( song );
        if( index === tracks.length - 1 ) {
            return tracks[0];
        }
        else {
            return tracks[ index + 1 ];
        }
    }
    
    /**
     * Retourne l'index dans le tableau de piste de la chanson
     * @param {Song} song
     * @returns {@exp;tracks@call;indexOf}
     */
    function findSong( song ) {
        return tracks.indexOf( song );
    }
    
    function findSongById( id ){
        var i;
        for( i =0; i<tracks.length; i++ ){
            if( tracks[i].id == id )
                return i;
        }
        return 0;
    }

    function getTHKW( song ) {
        var track;
        track = window.tomahkAPI.Track( song.artist, song.title, {
            width: 200,
            height: 200,
            disabledResolvers: [
                // options: "SoundCloud", "Officialfm", "Lastfm", "Jamendo", "Youtube", "Rdio", "SpotifyMetadata", "Deezer", "Exfm"
            ],
            handlers: {
                onloaded: function() {

                },
                onended: function() {
                    playing = false;
                    $("#btnNext").trigger("click");
                },
                onplayable: function() {
                    //console.log(track.connection+":\n  playable");
                    //TODO : activer btn play/pause, continuer la lecture si c'est une piste suivante
                    
                },
                onresolved: function(resolver, result) {
                    //console.log(track.connection+":\n  Track found: "+resolver+" - "+ result.track + " by "+result.artist);
                    if( track.song == currSong ){
                        if( !playing ) {
                            $("#btnPlay").button('reset');
                        }
                        else {
                            $("#btnPlay").button('pause');
                        }
                        $("body").trigger("hifi.player.playble");
                    }
                    else if ( track.song == getPrevious( currSong ) ) {
                        $("#btnPrev").button('reset');
                        $("#btnPrev").trigger("hifi.player.previous.playble");
                    }
                    else if ( track.song == getNext( currSong ) ) {
                        $("#btnNext").button('reset');
                        $("#btnNext").trigger("hifi.player.next.playble");
                    }
                },
                ontimeupdate: function(timeupdate) {
                    var currentTime = timeupdate.currentTime;
                    var duration = timeupdate.duration;
                    currentTime = parseInt(currentTime);
                    duration = parseInt(duration);

                    //C'est ici qu'il faut gérer si on veut faire une bare de progression
                    if( track.song == currSong ){
                        $( "#slider" ).slider( {
                            min : 0,
                            max : duration,
                            value : currentTime,
                            slide: function( event, ui ) {
                                track.seek( ui.value );
                            },
                            range: "min",
                            animate: true,
                            disabled: false
                        } );
                        //update times
                        $('.currTime').html( formatTime(currentTime) );
                        $('.totalTime').html( formatTime(duration) );
                    }
                }
            }
        });
        //add the song object in the THK var
        track.song = song;
        return track;
    }

    function renderTrack( track, elt ) {
        elt.html(track.render());
    }
    
    function formatTime( sec ) {
        if( isNaN(sec) ){
            return "--:--";
        }
        var s = sec % 60;
        var m = (sec - s) / 60;
        
        if( s < 10 )
            s = "0"+s;
        if( m < 10 )
            m = "0"+m;
        
        return m+":"+s;
    }
}
