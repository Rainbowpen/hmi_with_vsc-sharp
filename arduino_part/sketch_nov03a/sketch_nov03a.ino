
#include <Adafruit_Sensor.h>
#include <DHT.h>
#include <DHT_U.h>

#define DHTPIN 2     // Digital pin connected to the DHT sensor 
// Feather HUZZAH ESP8266 note: use pins 3, 4, 5, 12, 13 or 14 --
// Pin 15 can work but DHT must be disconnected during program upload.


#define DHTTYPE    DHT22     // DHT 22 (AM2302)
DHT_Unified dht(DHTPIN, DHTTYPE);

uint32_t delayMS;
int num_t = 0;
char num_s[6];
char flag_connect = 0;
//char str_temp[20];
void setup() {
  Serial.begin(115200);
  // Initialize device.
  dht.begin();
  sensor_t sensor;
  dht.temperature().getSensor(&sensor);
  dht.humidity().getSensor(&sensor);
  delayMS = sensor.min_delay / 1000;
}

void loop() {
  
  if(Serial.available()){
    char sread = Serial.read();
    if(sread == 'S'){
        flag_connect = 1;
    }else if(sread == 'D'){
        flag_connect = 0;
    } 
  }

  if(flag_connect == 1){
    // Delay between measurements.
    delay(delayMS);
    // Get temperature event and print its value.
    sensors_event_t event;
    dht.temperature().getEvent(&event);
    if (isnan(event.temperature)) {
      Serial.println(F("T00000E"));
    }
    else {
      //dtostrf(float(event.temperature), 4, 2, str_temp);
      sprintf(num_s,"T%03dE", int(event.temperature*10));
      Serial.println(num_s);
    }
  
    delay(100);
    
    // Get humidity event and print its value.
    dht.humidity().getEvent(&event);
    if (isnan(event.relative_humidity)) {
      Serial.println(F("H00000E"));
    }
    else {
      //dtostrf(float(event.relative_humidity), 4, 2, str_temp);
      sprintf(num_s,"H%03dE", int(event.relative_humidity*10));
      Serial.println(num_s);
    }
  }
}
