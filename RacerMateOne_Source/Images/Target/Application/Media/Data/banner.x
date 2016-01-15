xof 0303txt 0032
template VertexDuplicationIndices {
 <b8d65549-d7c9-4995-89cf-53a9a8b031e3>
 DWORD nIndices;
 DWORD nOriginalVertices;
 array DWORD indices[nIndices];
}

template EffectParamDWord {
 <e13963bc-ae51-4c5d-b00f-cfa3a9d97ce5>
 STRING ParamName;
 DWORD Value;
}

template FVFData {
 <b6e70a0e-8ef9-4e83-94ad-ecc8b0c04897>
 DWORD dwFVF;
 DWORD nDWords;
 array DWORD data[nDWords];
}

template EffectFloats {
 <f1cfe2b3-0de3-4e28-afa1-155a750a282d>
 DWORD nFloats;
 array FLOAT Floats[nFloats];
}

template EffectString {
 <d55b097e-bdb6-4c52-b03d-6051c89d0e42>
 STRING Value;
}

template EffectDWord {
 <622c0ed0-956e-4da9-908a-2af94f3ce716>
 DWORD Value;
}

template EffectInstance {
 <e331f7e4-0559-4cc2-8e99-1cec1657928f>
 STRING EffectFilename;
 [...]
}

template AnimTicksPerSecond {
 <9e415a43-7ba6-4a73-8743-b73d47e88476>
 DWORD AnimTicksPerSecond;
}

template VertexElement {
 <f752461c-1e23-48f6-b9f8-8350850f336f>
 DWORD Type;
 DWORD Method;
 DWORD Usage;
 DWORD UsageIndex;
}

template DeclData {
 <bf22e553-292c-4781-9fea-62bd554bdd93>
 DWORD nElements;
 array VertexElement Elements[nElements];
 DWORD nDWords;
 array DWORD data[nDWords];
}

template EffectParamFloats {
 <3014b9a0-62f5-478c-9b86-e4ac9f4e418b>
 STRING ParamName;
 DWORD nFloats;
 array FLOAT Floats[nFloats];
}

template EffectParamString {
 <1dbc4c88-94c1-46ee-9076-2c28818c9481>
 STRING ParamName;
 STRING Value;
}


Frame {
 

 FrameTransformMatrix {
  1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000;;
 }

 Mesh {
  83;
  1.950212;-0.125371;-0.029327;,
  2.029813;-0.125371;-0.029327;,
  2.029813;-0.125371;0.029327;,
  1.950212;-0.125371;0.029327;,
  1.950212;2.105590;-0.029327;,
  2.029813;2.105590;-0.029326;,
  2.029813;2.105590;0.029327;,
  1.950212;2.105590;0.029327;,
  -1.950212;-0.125371;-0.029327;,
  -2.029813;-0.125371;-0.029327;,
  -2.029813;-0.125371;0.029327;,
  -1.950212;-0.125371;0.029327;,
  -1.950212;2.105590;-0.029327;,
  -2.029813;2.105590;-0.029326;,
  -2.029813;2.105590;0.029327;,
  -1.950212;2.105590;0.029327;,
  0.756205;1.901038;0.000000;,
  -0.000000;1.874644;0.000000;,
  1.353208;1.953826;0.000000;,
  1.742158;1.814515;0.000000;,
  1.779172;1.633982;0.000000;,
  -0.756205;1.901038;0.000000;,
  -1.353209;1.953826;0.000000;,
  -1.742158;1.814515;0.000000;,
  -1.779172;1.633982;0.000000;,
  0.756205;1.901038;-0.029327;,
  -0.000000;1.874644;-0.029327;,
  1.353208;1.953826;-0.029326;,
  1.742158;1.814515;-0.029327;,
  1.779172;1.633982;-0.029327;,
  -0.756205;1.901038;-0.029327;,
  -1.353209;1.953826;-0.029327;,
  -1.742158;1.814515;-0.029326;,
  -1.779172;1.633982;-0.029327;,
  -0.000000;1.320373;0.000000;,
  1.980261;1.478736;0.000000;,
  -1.980262;1.478736;0.000000;,
  1.950212;2.059401;0.000000;,
  -1.950212;2.059401;0.000000;,
  1.393009;1.399555;0.000000;,
  -1.393009;1.399555;0.000000;,
  -0.000000;1.874644;0.000000;,
  0.756205;1.901038;0.000000;,
  -0.000000;1.874644;0.000000;,
  1.393009;1.399555;0.000000;,
  -0.000000;1.320373;0.000000;,
  1.393009;1.399555;0.000000;,
  0.756205;1.901038;0.000000;,
  1.353208;1.953826;0.000000;,
  1.393009;1.399555;0.000000;,
  1.742158;1.814515;0.000000;,
  1.353208;1.953826;0.000000;,
  1.950212;2.059401;0.000000;,
  1.779172;1.633982;0.000000;,
  1.980261;1.478736;0.000000;,
  1.393009;1.399555;0.000000;,
  1.742158;1.814515;0.000000;,
  1.779172;1.633982;0.000000;,
  1.353208;1.953826;0.000000;,
  1.742158;1.814515;0.000000;,
  1.393009;1.399555;0.000000;,
  1.393009;1.399555;0.000000;,
  -0.000000;1.874644;0.000000;,
  -0.756205;1.901038;0.000000;,
  -0.000000;1.874644;0.000000;,
  -1.393009;1.399555;0.000000;,
  -1.393009;1.399555;0.000000;,
  -0.000000;1.320373;0.000000;,
  -0.756205;1.901038;0.000000;,
  -1.353209;1.953826;0.000000;,
  -1.393009;1.399555;0.000000;,
  -1.742158;1.814515;0.000000;,
  -1.950212;2.059401;0.000000;,
  -1.353209;1.953826;0.000000;,
  -1.980262;1.478736;0.000000;,
  -1.779172;1.633982;0.000000;,
  -1.393009;1.399555;0.000000;,
  -1.742158;1.814515;0.000000;,
  -1.353209;1.953826;0.000000;,
  -1.779172;1.633982;0.000000;,
  -1.742158;1.814515;0.000000;,
  -1.393009;1.399555;0.000000;,
  -1.393009;1.399555;0.000000;;
  66;
  3;0,4,5;,
  3;0,5,1;,
  3;1,5,6;,
  3;1,6,2;,
  3;3,2,6;,
  3;3,6,7;,
  3;0,3,7;,
  3;0,7,4;,
  3;4,7,6;,
  3;4,6,5;,
  3;8,9,13;,
  3;8,13,12;,
  3;9,10,14;,
  3;9,14,13;,
  3;11,15,14;,
  3;11,14,10;,
  3;8,12,15;,
  3;8,15,11;,
  3;12,13,14;,
  3;12,14,15;,
  3;26,17,16;,
  3;26,16,25;,
  3;25,16,18;,
  3;25,18,27;,
  3;28,37,19;,
  3;27,18,37;,
  3;29,20,35;,
  3;29,28,19;,
  3;29,19,20;,
  3;26,30,21;,
  3;26,21,17;,
  3;30,31,22;,
  3;30,22,21;,
  3;32,23,38;,
  3;31,38,22;,
  3;36,24,33;,
  3;33,24,23;,
  3;33,23,32;,
  3;39,42,41;,
  3;39,43,34;,
  3;44,45,26;,
  3;46,26,25;,
  3;47,39,48;,
  3;25,27,49;,
  3;50,52,51;,
  3;28,27,37;,
  3;39,54,53;,
  3;55,29,35;,
  3;39,57,56;,
  3;39,59,58;,
  3;60,27,28;,
  3;61,28,29;,
  3;40,34,62;,
  3;40,64,63;,
  3;65,30,26;,
  3;66,26,67;,
  3;68,69,40;,
  3;30,70,31;,
  3;71,73,72;,
  3;32,38,31;,
  3;40,75,74;,
  3;76,36,33;,
  3;40,78,77;,
  3;40,80,79;,
  3;81,33,32;,
  3;82,32,31;;

  MeshNormals {
   83;
   -0.593198;0.000000;-0.805057;,
   0.827475;0.000000;-0.561502;,
   0.593198;0.000000;0.805057;,
   -0.345705;-0.000000;0.938343;,
   -0.592667;0.042293;-0.804336;,
   0.345678;0.012334;-0.938272;,
   0.593065;0.021161;0.804876;,
   -0.827385;0.014761;0.561441;,
   0.593200;0.000000;-0.805055;,
   -0.827476;-0.000000;-0.561501;,
   -0.593200;-0.000000;0.805055;,
   0.345706;-0.000000;0.938343;,
   0.592670;0.042293;-0.804335;,
   -0.345680;0.012334;-0.938272;,
   -0.593068;0.021161;0.804875;,
   0.827386;0.014761;0.561440;,
   -0.049987;0.998750;0.000000;,
   0.000000;1.000000;0.000000;,
   -0.117084;0.993122;0.000000;,
   0.976402;-0.215961;0.000000;,
   0.815727;0.578438;0.000000;,
   0.049987;0.998750;0.000000;,
   0.117084;0.993122;0.000000;,
   -0.976402;-0.215962;0.000000;,
   -0.815727;0.578438;0.000000;,
   -0.002123;0.024008;-0.999709;,
   0.000000;-0.015985;-0.999872;,
   -0.004638;0.026229;-0.999645;,
   0.017399;-0.014782;-0.999739;,
   0.065369;-0.055002;-0.996344;,
   0.002123;0.024008;-0.999709;,
   0.004638;0.026229;-0.999645;,
   -0.017399;-0.014782;-0.999739;,
   -0.065369;-0.055002;-0.996344;,
   0.000000;-0.000000;1.000000;,
   0.063711;-0.104950;-0.992435;,
   -0.063711;-0.104950;-0.992434;,
   0.064555;0.180236;-0.981503;,
   -0.064555;0.180236;-0.981503;,
   0.000000;0.000000;1.000000;,
   -0.000000;0.000000;1.000000;,
   0.000000;0.000000;1.000000;,
   0.000000;0.000000;1.000000;,
   0.000000;-0.000000;1.000000;,
   0.003003;-0.052836;-0.998599;,
   0.003003;-0.052836;-0.998599;,
   0.001951;-0.055910;-0.998434;,
   0.000000;0.000000;1.000000;,
   0.000000;0.000000;1.000000;,
   0.004642;-0.052503;-0.998610;,
   -0.000000;0.000000;1.000000;,
   -0.000000;0.000000;1.000000;,
   -0.000000;0.000000;1.000000;,
   -0.000000;0.000000;1.000000;,
   -0.000000;0.000000;1.000000;,
   0.021404;-0.158741;-0.987088;,
   0.000000;-0.000000;1.000000;,
   0.000000;-0.000000;1.000000;,
   -0.000000;0.000000;1.000000;,
   -0.000000;0.000000;1.000000;,
   -0.019419;-0.054217;-0.998340;,
   -0.067377;-0.013814;-0.997632;,
   0.000000;-0.000000;1.000000;,
   -0.000000;0.000000;1.000000;,
   -0.000000;0.000000;1.000000;,
   -0.001951;-0.055910;-0.998434;,
   -0.003003;-0.052836;-0.998599;,
   -0.003003;-0.052836;-0.998599;,
   -0.000000;0.000000;1.000000;,
   -0.000000;0.000000;1.000000;,
   -0.004642;-0.052503;-0.998610;,
   0.000000;-0.000000;1.000000;,
   0.000000;-0.000000;1.000000;,
   0.000000;-0.000000;1.000000;,
   -0.000000;0.000000;1.000000;,
   -0.000000;0.000000;1.000000;,
   -0.021404;-0.158741;-0.987088;,
   -0.000000;0.000000;1.000000;,
   -0.000000;0.000000;1.000000;,
   -0.000000;-0.000000;1.000000;,
   -0.000000;-0.000000;1.000000;,
   0.067377;-0.013814;-0.997632;,
   0.019419;-0.054217;-0.998340;;
   66;
   3;0,4,5;,
   3;0,5,1;,
   3;1,5,6;,
   3;1,6,2;,
   3;3,2,6;,
   3;3,6,7;,
   3;0,3,7;,
   3;0,7,4;,
   3;4,7,6;,
   3;4,6,5;,
   3;8,9,13;,
   3;8,13,12;,
   3;9,10,14;,
   3;9,14,13;,
   3;11,15,14;,
   3;11,14,10;,
   3;8,12,15;,
   3;8,15,11;,
   3;12,13,14;,
   3;12,14,15;,
   3;26,17,16;,
   3;26,16,25;,
   3;25,16,18;,
   3;25,18,27;,
   3;28,37,19;,
   3;27,18,37;,
   3;29,20,35;,
   3;29,28,19;,
   3;29,19,20;,
   3;26,30,21;,
   3;26,21,17;,
   3;30,31,22;,
   3;30,22,21;,
   3;32,23,38;,
   3;31,38,22;,
   3;36,24,33;,
   3;33,24,23;,
   3;33,23,32;,
   3;39,42,41;,
   3;39,43,34;,
   3;44,45,26;,
   3;46,26,25;,
   3;47,39,48;,
   3;25,27,49;,
   3;50,52,51;,
   3;28,27,37;,
   3;39,54,53;,
   3;55,29,35;,
   3;39,57,56;,
   3;39,59,58;,
   3;60,27,28;,
   3;61,28,29;,
   3;40,34,62;,
   3;40,64,63;,
   3;65,30,26;,
   3;66,26,67;,
   3;68,69,40;,
   3;30,70,31;,
   3;71,73,72;,
   3;32,38,31;,
   3;40,75,74;,
   3;76,36,33;,
   3;40,78,77;,
   3;40,80,79;,
   3;81,33,32;,
   3;82,32,31;;
  }

  MeshTextureCoords {
   83;
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.000000;1.000000;,
   0.690745;0.214571;,
   0.500000;0.250250;,
   0.841333;0.143214;,
   0.939441;0.331530;,
   0.948778;0.575572;,
   0.309255;0.214571;,
   0.158667;0.143214;,
   0.060559;0.331530;,
   0.051222;0.575572;,
   0.690745;0.214571;,
   0.500000;0.250250;,
   0.841333;0.143214;,
   0.939441;0.331530;,
   0.948778;0.575572;,
   0.309255;0.214571;,
   0.158667;0.143214;,
   0.060559;0.331530;,
   0.051222;0.575572;,
   0.500000;0.999501;,
   0.999501;0.785429;,
   0.000500;0.785429;,
   0.991921;0.000500;,
   0.008079;0.000500;,
   0.148628;0.892465;,
   0.851372;0.892465;,
   0.500000;0.250250;,
   0.309255;0.214571;,
   0.500000;0.250250;,
   0.851372;0.892465;,
   0.500000;0.999501;,
   0.851372;0.892465;,
   0.309255;0.214571;,
   0.158667;0.143214;,
   0.851372;0.892465;,
   0.060559;0.331530;,
   0.158667;0.143214;,
   0.008079;0.000500;,
   0.051222;0.575572;,
   0.000500;0.785429;,
   0.851372;0.892465;,
   0.060559;0.331530;,
   0.051222;0.575572;,
   0.158667;0.143214;,
   0.060559;0.331530;,
   0.851372;0.892465;,
   0.851372;0.892465;,
   0.500000;0.250250;,
   0.690745;0.214571;,
   0.500000;0.250250;,
   0.148628;0.892465;,
   0.148628;0.892465;,
   0.500000;0.999501;,
   0.690745;0.214571;,
   0.841333;0.143214;,
   0.148628;0.892465;,
   0.939442;0.331530;,
   0.991921;0.000500;,
   0.841333;0.143214;,
   0.999501;0.785429;,
   0.948778;0.575572;,
   0.148628;0.892465;,
   0.939442;0.331530;,
   0.841333;0.143214;,
   0.948778;0.575572;,
   0.939442;0.331530;,
   0.148628;0.892465;,
   0.148628;0.892465;;
  }

  MeshMaterialList {
   2;
   66;
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   0,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1,
   1;

   Material {
    0.498039;0.498039;0.498039;1.000000;;
    20.000000;
    0.898039;0.898039;0.898039;;
    0.000000;0.000000;0.000000;;
   }

   Material {
    0.537255;0.196078;0.196078;1.000000;;
    20.000000;
    0.898039;0.898039;0.898039;;
    0.000000;0.000000;0.000000;;

    TextureFilename {
     "banner.bmp";
    }
   }
  }

  VertexDuplicationIndices {
   83;
   41;
   0,
   1,
   2,
   3,
   4,
   5,
   6,
   7,
   8,
   9,
   10,
   11,
   12,
   13,
   14,
   15,
   16,
   17,
   18,
   19,
   20,
   21,
   22,
   23,
   24,
   25,
   26,
   27,
   28,
   29,
   30,
   31,
   32,
   33,
   34,
   35,
   36,
   37,
   38,
   39,
   40,
   17,
   16,
   17,
   39,
   34,
   39,
   16,
   18,
   39,
   19,
   18,
   37,
   20,
   35,
   39,
   19,
   20,
   18,
   19,
   39,
   39,
   17,
   21,
   17,
   40,
   40,
   34,
   21,
   22,
   40,
   23,
   38,
   22,
   36,
   24,
   40,
   23,
   22,
   24,
   23,
   40,
   40;
  }
 }
}