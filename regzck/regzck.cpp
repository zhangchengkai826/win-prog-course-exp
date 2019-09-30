#include "pch.h"
#include "regzck.h"

std::vector<KeyName> vSubKeyNames;
std::vector<RegValue> vRegValues;
std::vector<BYTE> vRegValDataBuf;

void __stdcall regQuery(HKEY hKey, KeyName **subKeyNames, int *pcSubKeys, RegValue **regValues, int *pcValues) {
	DWORD    cbName;   // size of name string (subkey)
	DWORD    cchValue;		// size of name string (value)
	TCHAR    achClass[MAX_PATH] = TEXT("");  // buffer for class name 
	DWORD    cchClassName = MAX_PATH;  // size of class string 
	DWORD    cSubKeys = 0;               // number of subkeys 
	DWORD    cbMaxSubKey;              // longest subkey size 
	DWORD    cchMaxClass;              // longest class string 
	DWORD    cValues;              // number of values for key 
	DWORD    cchMaxValue;          // longest value name 
	DWORD    cbMaxValueData;       // longest value data 
	DWORD    cbSecurityDescriptor; // size of security descriptor 
	FILETIME ftLastWriteTime;      // last write time 

	DWORD i, retCode;

	// Get the class name and the value count. 
	retCode = RegQueryInfoKey(
		hKey,                    // key handle 
		achClass,                // buffer for class name 
		&cchClassName,           // size of class string 
		NULL,                    // reserved 
		&cSubKeys,               // number of subkeys 
		&cbMaxSubKey,            // longest subkey size 
		&cchMaxClass,            // longest class string 
		&cValues,                // number of values for this key 
		&cchMaxValue,            // longest value name 
		&cbMaxValueData,         // longest value data 
		&cbSecurityDescriptor,   // security descriptor 
		&ftLastWriteTime);       // last write time 

	// Enumerate the subkeys, until RegEnumKeyEx fails.
	vSubKeyNames.resize(cSubKeys);
	int cRealSubKeys = 0;
	for (i = 0; i < cSubKeys; i++)
	{
		cbName = MAX_KEY_LENGTH;
		retCode = RegEnumKeyEx(hKey, i,
			vSubKeyNames[cRealSubKeys].achKey,
			&cbName,
			NULL,
			NULL,
			NULL,
			&ftLastWriteTime);
		if (retCode == ERROR_SUCCESS)
		{
			cRealSubKeys++;
		}
	}
	*subKeyNames = vSubKeyNames.data();
	*pcSubKeys = cRealSubKeys;
	
	vRegValues.resize(cValues);
	vRegValDataBuf.resize((UINT64)cValues * cbMaxValueData);
	int cRealValues = 0;
	// Enumerate the key values. 
	for (i = 0; i < cValues; i++)
	{
		cchValue = MAX_VALUE_NAME;
		vRegValues[cRealValues].data = vRegValDataBuf.data() + (UINT64)cbMaxValueData * cRealValues;
		vRegValues[cRealValues].cbData = cbMaxValueData;
		retCode = RegEnumValue(hKey, i,
			vRegValues[cRealValues].name,
			&cchValue,
			NULL,
			&vRegValues[cRealValues].type,
			vRegValues[cRealValues].data,
			&vRegValues[cRealValues].cbData);

		if (retCode == ERROR_SUCCESS)
		{
			cRealValues++;
		}
	}
	*regValues = vRegValues.data();
	*pcValues = cRealValues;
}

void __stdcall regOpen(HKEY parentKey, KeyName name, HKEY* output) {
	if (RegOpenKeyEx(parentKey, name.achKey, 0, KEY_ALL_ACCESS, output) != ERROR_SUCCESS) {
		*output = 0;
	}
}

void __stdcall regNewKey(HKEY parentKey, KeyName name, HKEY *newKey) {
	auto retCode = RegCreateKey(parentKey, name.achKey, newKey);
	if (retCode != ERROR_SUCCESS) {
		*newKey = 0;
	}
}

void __stdcall regDelKey(HKEY parentKey, KeyName name) {
	auto retCode = RegDeleteKey(parentKey, name.achKey);
	if (retCode != ERROR_SUCCESS) {
		// log err
	}
}