#include "resource.h"
#include "windows.h"
#include "strsafe.h"
INT_PTR CALLBACK  DiProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam);
void populate(HWND hwnd, WPARAM wParam, LPARAM lParam);
void PopUp(HWND hwnd);

int WINAPI WinMain(HINSTANCE hInstance,
	HINSTANCE hprevInstance,
	LPSTR lpCmdLine,
	int nShowCmd)
{
	DialogBox(hInstance, MAKEINTRESOURCE(IDD_DIALOG1), NULL, DiProc);
	return 0;
}

INT_PTR CALLBACK DiProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	UNREFERENCED_PARAMETER(lParam);
	switch (message)
	{
	case WM_INITDIALOG:
		populate(hwnd, wParam, lParam);
		return (INT_PTR)TRUE;

	case WM_COMMAND:
		if (HIWORD(wParam) == BN_CLICKED)
		{
			switch (LOWORD(wParam))
			{
			case IDOK:
				PopUp(hwnd);
				break;
			case IDCANCEL:
				EndDialog(hwnd, LOWORD(wParam));
				break;
			}
		}
		break;
	}
	return (INT_PTR)FALSE;
}



void populate(HWND hwnd, WPARAM wParam, LPARAM lParam)
{
	SendMessage(GetDlgItem(hwnd, UNI_EDIT), EM_LIMITTEXT, 100, 0);
	SendMessage(GetDlgItem(hwnd, ADD_EDIT), EM_LIMITTEXT, 100, 0);
	SendMessage(GetDlgItem(hwnd, CMB_BOX), CB_ADDSTRING, 0, L"3-letnie");
	SendMessage(GetDlgItem(hwnd, CMB_BOX), CB_ADDSTRING, 0, L"II stopnia");
	SendMessage(GetDlgItem(hwnd, CMB_BOX), CB_ADDSTRING, 0, L"jakieś coś");
	SendMessage(GetDlgItem(hwnd, CMB_BOX), CB_SETCURSEL, 0, 0);

}

void PopUp(HWND hwnd)
{
	TCHAR s0[301] = "";
	TCHAR s1[101];
	TCHAR s2[101];
	TCHAR s3[101];
	TCHAR s4[101];
	GetWindowText(GetDlgItem(hwnd, UNI_EDIT), s1, 101);
	GetWindowText(GetDlgItem(hwnd, ADD_EDIT), s2, 101);
	StringCchCat(s0, 300, s1);
	StringCchCat(s0, 300, "\n");
	StringCchCat(s0, 300, s2);
	StringCchCat(s0, 300, "\n");
	int selected = SendMessage(GetDlgItem(hwnd, CMB_BOX), CB_GETCURSEL, 0, 0);
	SendMessage(GetDlgItem(hwnd, CMB_BOX), CB_GETLBTEXT, selected, s3);
	StringCchCat(s0, 300, s3);
	StringCchCat(s0, 300, "\n");
	if (IsDlgButtonChecked(hwnd, ID_D_CHECK) && IsDlgButtonChecked(hwnd, ID_U_CHECK))
		StringCchCat(s0, 300, "Dzienne, uzupełniające");
	else if (IsDlgButtonChecked(hwnd, ID_D_CHECK)) StringCchCat(s0, 300, "Dzienne");
	else if (IsDlgButtonChecked(hwnd, ID_U_CHECK)) StringCchCat(s0, 300, "Uzupełniające");
	MessageBox(0, s0, "Uczelnia", 0);

}