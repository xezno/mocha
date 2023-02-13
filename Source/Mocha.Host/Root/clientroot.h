#pragma once
#include <Root/root.h>

class ClientRoot : public Root
{
protected:
	bool GetQuitRequested() override;

public:
	ClientRoot() { Globals::m_executingRealm = REALM_CLIENT; }
};