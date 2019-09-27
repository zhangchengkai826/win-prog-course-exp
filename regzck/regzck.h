#pragma once

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files
#include <windows.h>

const int MAX_KEY_LENGTH = 255;
const int MAX_VALUE_NAME = 16383;

typedef struct tagKeyName {
	TCHAR    achKey[MAX_KEY_LENGTH];   // buffer for subkey name
} KeyName;

typedef struct tagRegValue {
	TCHAR    name[MAX_VALUE_NAME];
} RegValue;

extern "C" {
	void __stdcall regList(HKEY hKey, KeyName** subKeyNames, int* pcSubKeys, RegValue** regValues, int* pcValues);
	void __stdcall regOpen(HKEY parentKey, KeyName name, HKEY* output);
}