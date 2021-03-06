#include <Windows.h>
#include <strsafe.h>
#define ID_CANCEL 1234
#define ID_ACCEPT 2345
#define ID_D_CHECK 3456
#define ID_U_CHECK 4567

LRESULT CALLBACK WindowProcedure(HWND, UINT, WPARAM, LPARAM);
TCHAR * PopUp();
void populate(HWND, WPARAM, LPARAM);

char szClassName[] = "Wybór uczelni";
HWND uniEdit;
HWND addEdit;
HWND mainWind;
HWND cmbBox;

int WINAPI WinMain(HINSTANCE hInstance,
	HINSTANCE hprevInstance,
	LPSTR lpCmdLine,
	int nShowCmd)
{

	MSG messages;
	WNDCLASSEX wincl;
	wincl.hInstance = hInstance;
	wincl.lpszClassName = szClassName;
	wincl.lpfnWndProc = WindowProcedure;
	wincl.style = CS_DBLCLKS;
	wincl.cbSize = sizeof(WNDCLASSEX);
	wincl.hIcon = LoadIcon(NULL, IDI_APPLICATION);
	wincl.hIconSm = LoadIcon(NULL, IDI_APPLICATION);
	wincl.hCursor = LoadCursor(NULL, IDC_ARROW);
	wincl.lpszMenuName = NULL;
	wincl.cbClsExtra = 0;
	wincl.cbWndExtra = 0;
	wincl.hbrBackground = (HBRUSH)GetStockObject(LTGRAY_BRUSH);
	if (!RegisterClassEx(&wincl)) return 0;
	mainWind = CreateWindowEx(
		0, szClassName, "Wybór uczelni",
		WS_CAPTION | WS_SYSMENU | WS_MINIMIZEBOX,
		CW_USEDEFAULT, CW_USEDEFAULT,
		600, 400, HWND_DESKTOP, NULL,
		hInstance, NULL);
	ShowWindow(mainWind, nShowCmd);
	while (GetMessage(&messages, NULL, 0, 0))
	{
		TranslateMessage(&messages);
		DispatchMessage(&messages);
	}
	
	return messages.wParam;
}

LRESULT CALLBACK WindowProcedure(HWND hwnd, UINT message,
	WPARAM wParam, LPARAM lParam)
{
	switch (message)
	{
	case WM_CREATE:
	{
		populate(hwnd, wParam, lParam);
		break;
	}
	case WM_COMMAND:
		if (HIWORD(wParam) == BN_CLICKED)
		{
			switch (LOWORD(wParam))
			{
			case ID_ACCEPT:
				PopUp(hwnd);
				break;
			case ID_CANCEL:
				PostQuitMessage(0);
				break;
			case ID_D_CHECK:
			case ID_U_CHECK:
				if (IsDlgButtonChecked(hwnd, LOWORD(wParam))) CheckDlgButton(hwnd, LOWORD(wParam), BST_UNCHECKED);
				else CheckDlgButton(hwnd, LOWORD(wParam), BST_CHECKED);
				break;
			}
		}
		break;
	case WM_DESTROY:
	{
		PostQuitMessage(0);
		break;
	}
	default: return DefWindowProc(hwnd, message, wParam, lParam);
	}

}

TCHAR * PopUp(HWND hwnd)
{
	TCHAR s0[301] = "";
	TCHAR s1[101];
	TCHAR s2[101];
	TCHAR s3[101];
	TCHAR s4[101];
	GetWindowText(uniEdit, s1, 101);
	GetWindowText(addEdit, s2, 101);
	StringCchCat(s0, 300, s1);
	StringCchCat(s0, 300, "\n");
	StringCchCat(s0, 300, s2);
	StringCchCat(s0, 300, "\n");
	int selected = SendMessage(cmbBox, CB_GETCURSEL, 0, 0);
	SendMessage(cmbBox, CB_GETLBTEXT, selected, s3);
	StringCchCat(s0, 300, s3);
	StringCchCat(s0, 300, "\n");
	if (IsDlgButtonChecked(hwnd, ID_D_CHECK) && IsDlgButtonChecked(hwnd, ID_U_CHECK))
		StringCchCat(s0, 300, "Dzienne, uzupełniające");
	else if (IsDlgButtonChecked(hwnd, ID_D_CHECK)) StringCchCat(s0, 300, "Dzienne");
	else if (IsDlgButtonChecked(hwnd, ID_U_CHECK)) StringCchCat(s0, 300, "Uzupełniające");
	MessageBox(0, s0, "Uczelnia", 0);
	return s0;
}
void populate(HWND hwnd, WPARAM wParam, LPARAM lParam)
{
	CreateWindow("BUTTON", "Uczelnia",
		WS_CHILD | WS_VISIBLE | WS_GROUP | BS_GROUPBOX,
		10, 10, 550, 100, hwnd, (HMENU)NULL,
		((LPCREATESTRUCT)lParam)->hInstance,
		NULL);
	CreateWindow("STATIC", "Nazwa:",
		WS_CHILD | WS_VISIBLE,
		30, 40, 60, 26, hwnd, (HMENU)NULL,
		((LPCREATESTRUCT)lParam)->hInstance,
		NULL);
	CreateWindow("STATIC", "Adres:",
		WS_CHILD | WS_VISIBLE,
		30, 70, 60, 26, hwnd, (HMENU)NULL,
		((LPCREATESTRUCT)lParam)->hInstance,
		NULL);
	uniEdit = CreateWindow("EDIT", "Nazwa uczelni",
		WS_CHILD | WS_VISIBLE | WS_BORDER,
		90, 40, 450, 26, hwnd, (HMENU)NULL,
		((LPCREATESTRUCT)lParam)->hInstance,
		NULL);
	addEdit = CreateWindow("EDIT", "Adres uczelni",
		WS_CHILD | WS_VISIBLE | WS_BORDER,
		90, 70, 450, 26, hwnd, (HMENU)NULL,
		((LPCREATESTRUCT)lParam)->hInstance,
		NULL);
	SendMessage(uniEdit, EM_LIMITTEXT, 100, 0);
	SendMessage(addEdit, EM_LIMITTEXT, 100, 0);
	CreateWindow("BUTTON", "Rodzaj studiów",
		WS_CHILD | WS_VISIBLE | WS_GROUP | BS_GROUPBOX,
		10, 130, 550, 100, hwnd, (HMENU)NULL,
		((LPCREATESTRUCT)lParam)->hInstance,
		NULL);
	CreateWindow("STATIC", "Cykl nauki:",
		WS_CHILD | WS_VISIBLE,
		30, 160, 80, 26, hwnd, (HMENU)NULL,
		((LPCREATESTRUCT)lParam)->hInstance,
		NULL);

	cmbBox = CreateWindow("COMBOBOX", "Dunno if it matters",
		CBS_AUTOHSCROLL | WS_TABSTOP | WS_GROUP | WS_CHILD | CBS_DROPDOWNLIST | WS_VISIBLE,
		120, 160, 400, 27*3, hwnd, (HMENU)NULL,
		((LPCREATESTRUCT)lParam)->hInstance,
		NULL);
	SendMessage(cmbBox, CB_ADDSTRING, 0, "3-letnie");
	SendMessage(cmbBox, CB_ADDSTRING, 0, "II stopnia");
	SendMessage(cmbBox, CB_ADDSTRING, 0, "jakieś coś");

	SendMessage(cmbBox, CB_SETCURSEL, 0, 0);
	CreateWindow("BUTTON", "Dzienne",
		WS_CHILD | WS_VISIBLE | BS_CHECKBOX,
		30, 190, 100, 26, hwnd, (HMENU)ID_D_CHECK,
		((LPCREATESTRUCT)lParam)->hInstance,
		NULL);
	CreateWindow("BUTTON", "Uzupełniające",
		WS_CHILD | WS_VISIBLE | BS_CHECKBOX,
		160, 190, 150, 26, hwnd, (HMENU)ID_U_CHECK,
		((LPCREATESTRUCT)lParam)->hInstance,
		NULL);
	CreateWindow("BUTTON", "Akceptuj",
		BS_MULTILINE | WS_CHILD | WS_TABSTOP | WS_GROUP | WS_VISIBLE,
		120, 250, 100, 26,
		hwnd, (HMENU)ID_ACCEPT,
		((LPCREATESTRUCT)lParam)->hInstance,
		NULL);
	CreateWindow("BUTTON", "Anuluj",
		BS_MULTILINE | WS_CHILD | WS_TABSTOP | WS_GROUP | WS_VISIBLE,
		420, 250, 100, 26,
		hwnd, (HMENU)ID_CANCEL,
		((LPCREATESTRUCT)lParam)->hInstance,
		NULL);
}