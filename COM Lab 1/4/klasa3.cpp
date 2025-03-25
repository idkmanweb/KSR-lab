#include<windows.h>
#include"klasa3.h"
#include<stdio.h>

extern volatile int usageCount;


Klasa3::Klasa3() {
	usageCount++;
	m_ref = 0;
	};


Klasa3::~Klasa3() {	
	usageCount--;
	};


ULONG STDMETHODCALLTYPE Klasa3::AddRef() {
	InterlockedIncrement(reinterpret_cast<volatile LONG*>(&m_ref));
	return m_ref;
	};


ULONG STDMETHODCALLTYPE Klasa3::Release() {
	ULONG rv = InterlockedDecrement(reinterpret_cast<volatile LONG*>(&m_ref));
	if (rv == 0) delete this;
	return rv;
	};


HRESULT STDMETHODCALLTYPE Klasa3::QueryInterface(REFIID iid, void **ptr) {
	if(ptr == NULL) return E_POINTER;
	if(IsBadWritePtr(ptr, sizeof(void *))) return E_POINTER;
	*ptr = NULL;
	if(iid == IID_IUnknown) *ptr = this;
	if(iid == IID_IKlasa3) *ptr = this;
	if(*ptr != NULL) { AddRef(); return S_OK; };
	return E_NOINTERFACE;
	};

HRESULT STDMETHODCALLTYPE Klasa3::Test(const char *napis){
	if (napis == NULL) {
		return E_POINTER;
		}

	printf("%s\n", napis);
	return S_OK;
	};

