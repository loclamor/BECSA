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
    
    body.append("<div class='btn-group'>"
                + "<button id='btnPrev' class='btn btn-default'>Prev</button>"
                + "<button id='btnPlay' class='btn btn-default'>Play</button>"
                + "<button id='btnNext' class='btn btn-default'>Next</button>"
              + "</div>");
    
    $("#btnPlay").click(function(){
        if( playing ) {
            playing = false;
            thkCurr.pause();
        }
        else {
            playing = true;
             thkCurr.play();
        }
    });
    
    $("#btnPrev").click(function(){
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
    });
    
    $("#btnNext").click(function(){
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
    });
    
//recuperation des tracks
    $.getJSON( getControllerActionUrl("hifi", "lister"), function( data ){
        if( data.code < 300 ) {
            tracks = data.songs;
            $.each( data.songs, function( key, val ) {
                
                console.log( val.id + ' : ' + val.artist + ' - ' + val.title + ' - ' + findSong( val ) + ' - ' + getPrevious( val ).title + ' - ' + getNext( val ).title);
            });
            //generate players
            currSong = tracks[0];
            thkCurr = getTHKW( tracks[0] );
            renderTrack( thkCurr, $(".curr") );
            
            thkPrev = getTHKW( getPrevious( tracks[0] ) );
            renderTrack( thkPrev, $(".prev") );
            
            thkNext = getTHKW( getNext( tracks[0] ) );
            renderTrack( thkNext, $(".next") );
            
            list.slideDown(500);
        }
        else {
            notify("danger", data.message, "Error", 5000);
        }
    });
    
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
                },
                ontimeupdate: function(timeupdate) {
                    var currentTime = timeupdate.currentTime;
                    var duration = timeupdate.duration;
                    currentTime = parseInt(currentTime);
                    duration = parseInt(duration);

                    //console.log(track.connection+":\n  Time update: "+currentTime + " "+duration);
                }
            }
        });
        return track;
    }

    function renderTrack( track, elt ) {
        elt.html(track.render());
    }

}
