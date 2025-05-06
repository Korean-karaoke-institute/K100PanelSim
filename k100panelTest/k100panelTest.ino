#define bps 38400
#define K100_PANEL_DATA_SIZE 0x36
#define SHOW_DOT_DATA true
byte Panel_check_byte[] = {0xaa, 0x71, 0x1b};

void setup() {
  Serial.begin(bps);
  Serial1.begin(bps);
  Serial2.begin(bps);
}

void loop() {
  if (Serial1.available()) {
    byte b = Serial1.read();
    
    if (b == 0x55) {
      byte temp[K100_PANEL_DATA_SIZE - 1] = {0};
      Serial1.readBytes(temp, K100_PANEL_DATA_SIZE - 1);
      if (temp[K100_PANEL_DATA_SIZE - 2] == 0xFF && SHOW_DOT_DATA) {
        Serial1.write(b);
        Serial1.write(temp,K100_PANEL_DATA_SIZE - 1);
        Serial.print("DOT:");
        Serial.print(0x55);
        Serial.print(",");
        for (int i = 0; i < K100_PANEL_DATA_SIZE - 1; i++) {
          Serial.print(temp[i]);
          if (i < K100_PANEL_DATA_SIZE - 2)
            Serial.print(",");
        }
        Serial.println();
      }

    }
    else if(b == 0xdd){
      byte temp[7] = {0};
      byte sum = 0;
      byte recvsum = 0;
      Serial1.readBytes(temp, 7);
      recvsum = temp[6];
      for(int i=0; i<6; i++){
        sum = sum + temp[i];
      }
      if(sum == recvsum){
        Serial1.write(b);
        Serial1.write(temp,7);
        Serial.print("FNDS:");
        Serial.print(0xdd);
        Serial.print(",");
        for(int i=0; i<7; i++){
          Serial.print(temp[i]);
          if (i < 6)
            Serial.print(",");
        }
        Serial.println();
      }
    }
    else if(b == 0xee){
      byte temp[4] = {0};
      Serial1.readBytes(temp,4);
      Serial1.write(b);
      Serial1.write(temp,4);     
      Serial.print("FNDC:");      
      Serial.print(0xEE);
      Serial.print(",");
      for(int i=0; i<4;i++){
        Serial.print(temp[i]);
        if (i < 3)
            Serial.print(",");
      }
      Serial.println();
    }
    else{
      Serial1.write(b);
    }
    
  }
  if(Serial.available()){
    byte b = Serial.read();
    Serial2.write(b);
  }
  if(Serial2.available()){
    byte b = Serial2.read();
    Serial2.write(b);
  }
}
