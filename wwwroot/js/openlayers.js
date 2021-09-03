

var vehicleCount = await fetch("https://localhost:44317/Map/MetaData").then(response => response.text());

var styleArray = [
    {
        'Point': new ol.style.Style({
            image: new ol.style.Circle({
                fill: new ol.style.Fill({
                    color: 'rgba(255,255,255)'
                }),
                radius: 20,
                stroke: new ol.style.Stroke({
                    color: '#0ff',
                    width: 3
                }),
            }),
            text: new ol.style.Text({
                text: "1",
                scale: "3"
            })
        }),
        'LineString': new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: '#0ff',
                width: 7
            })
        }),
        'MultiLineString': new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: '#0ff',
                width: 7
            })
        })
    },
    {
        'Point': new ol.style.Style({
            image: new ol.style.Circle({
                fill: new ol.style.Fill({
                    color: 'rgba(255,255,255)'
                }),
                radius: 20,
                stroke: new ol.style.Stroke({
                    color: '#0f0',
                    width: 3
                }),
            }),
            text: new ol.style.Text({
                text: "2",
                scale: "3"
            })
        }),
        'LineString': new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: '#0f0',
                width: 7
            })
        }),
        'MultiLineString': new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: '#0f0',
                width: 7
            })
        })
    },
    {
        'Point': new ol.style.Style({
            image: new ol.style.Circle({
                fill: new ol.style.Fill({
                    color: 'rgba(255,255,255)'
                }),
                radius: 20,
                stroke: new ol.style.Stroke({
                    color: '#FF0000',
                    width: 3
                }),
            }),
            text: new ol.style.Text({
                text: "3",
                scale: "3"
            })
        }),
        'LineString': new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: '#FF0000',
                width: 7
            })
        }),
        'MultiLineString': new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: '#FF0000',
                width: 7
            })
        })
    }
];




//Prepare Layer Array
var layerArray = [
    new ol.layer.Tile({
        source: new ol.source.OSM()
    })
]

for (let i = 0; i < vehicleCount; i++) {

    var url = await fetch("https://localhost:44317/Map/GPX/" + i).then(response => response.text());

    //Route Layer 
    var vectorLayer = new ol.layer.Vector({
        source: new ol.source.Vector({
            url: url,
            format: new ol.format.GPX(),
        }),
        style: function (feature) {
            return styleArray[i][feature.getGeometry().getType()];
        },
    })
    layerArray.push(vectorLayer);
}



/*const place = [39.9074, 32.7904];
const point = new ol.geom.Point(place);

var vectorLayer = new ol.layer.Vector(
    {
        source: new ol.source.Vector({
            features: ol.proj.fromLonLat([32.866287, 39.925533]),
        }),
        style: new ol.style.Style({
            image: new ol.style.Circle({
                radius: 9,
                fill: new Fill({ color: 'red' })
            })
        })
    })*/


/*layerArray.push(vectorLayer);
*/

//https://openlayers.org/en/v3.20.1/examples/draw-shapes.html
//https://openlayers.org/en/v3.20.1/examples/popup.html

//

// Create Map
var map = new ol.Map({
    target: document.getElementById('map'),

    layers: layerArray,

    view: new ol.View({
        center: ol.proj.fromLonLat([32.866287, 39.925533]),
        zoom: 10
    })
});



var displayFeatureInfo = function (pixel) {
    var features = [];
    map.forEachFeatureAtPixel(pixel, function (feature) {
        features.push(feature);
    });
    if (features.length > 0) {
        var info = [];
        var i, ii;
        for (i = 0, ii = features.length; i < ii; ++i) {
            info.push(features[i].get('desc'));
        }
        document.getElementById('info').innerHTML = info.join(', ') || '(unknown)';
        map.getTarget().style.cursor = 'pointer';
    } else {
        document.getElementById('info').innerHTML = '&nbsp;';
        map.getTarget().style.cursor = '';
    }


};

/*map.on('pointermove', function (evt) {
    if (evt.dragging) {  
        return;
    }
    var pixel = map.getEventPixel(evt.originalEvent);
    displayFeatureInfo(pixel);
});*/

map.on('click', function (evt) {
    displayFeatureInfo(evt.pixel);
});