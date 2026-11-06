using BookManagement.BLL;
using BookManagement.Entity;
using System.Globalization;

namespace BookManagement.GUI;

public sealed class MainForm : Form
{
    private readonly BookService _bookService = new();
    private readonly ComboBox cboTheLoai = new();
    private readonly TextBox txtMaSach = new();
    private readonly TextBox txtTenSach = new();
    private readonly DateTimePicker dtpNgayNhap = new();
    private readonly TextBox txtSoLuong = new();
    private readonly TextBox txtDonGia = new();
    private readonly DataGridView dgvSach = new();
    private readonly Button btnThem = new();
    private readonly Button btnSua = new();
    private readonly Button btnXoa = new();

    public MainForm()
    {
        Text = "Quản lý sách – 24110237 – {Phạm Quang Huy}";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(920, 640);
        Size = new Size(1120, 760);
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(245, 247, 250);
        KeyPreview = true;
        AcceptButton = btnThem;

        BuildLayout();
        ConfigureGrid();
        WireEvents();
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            ColumnCount = 1,
            RowCount = 3,
            BackColor = BackColor
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 255));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(root);

        var header = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(31, 78, 121),
            Padding = new Padding(18, 8, 12, 8)
        };
        header.Controls.Add(new Label
        {
            Text = Text,
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 17F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        });
        root.Controls.Add(header, 0, 0);

        var entryArea = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Padding = new Padding(0, 12, 0, 12)
        };
        entryArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        entryArea.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155));
        root.Controls.Add(entryArea, 0, 1);

        var information = new GroupBox
        {
            Text = "Thông tin sách",
            Dock = DockStyle.Fill,
            Padding = new Padding(14),
            BackColor = Color.White
        };
        entryArea.Controls.Add(information, 0, 0);

        var fields = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 4,
            Padding = new Padding(6, 12, 6, 4)
        };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (var row = 0; row < 4; row++)
        {
            fields.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
        }
        information.Controls.Add(fields);

        ConfigureInput(cboTheLoai);
        cboTheLoai.Name = "cboTheLoai";
        cboTheLoai.DropDownStyle = ComboBoxStyle.DropDownList;
        ConfigureInput(txtMaSach);
        txtMaSach.Name = "txtMaSach";
        ConfigureInput(txtTenSach);
        txtTenSach.Name = "txtTenSach";
        ConfigureInput(txtSoLuong);
        txtSoLuong.Name = "txtSoLuong";
        ConfigureInput(txtDonGia);
        txtDonGia.Name = "txtDonGia";
        dtpNgayNhap.Dock = DockStyle.Fill;
        dtpNgayNhap.Name = "dtpNgayNhap";
        dtpNgayNhap.Format = DateTimePickerFormat.Custom;
        dtpNgayNhap.CustomFormat = "dd/MM/yyyy";
        dtpNgayNhap.MaxDate = DateTime.Today;
        dtpNgayNhap.Margin = new Padding(4, 6, 12, 4);

        AddField(fields, "Thể loại", cboTheLoai, 0, 0);
        AddField(fields, "Mã sách", txtMaSach, 0, 2);
        AddField(fields, "Tên sách", txtTenSach, 1, 0, 3);
        AddField(fields, "Ngày nhập", dtpNgayNhap, 2, 0);
        AddField(fields, "Số lượng", txtSoLuong, 2, 2);
        AddField(fields, "Đơn giá", txtDonGia, 3, 0);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(12, 10, 0, 0)
        };
        entryArea.Controls.Add(actions, 1, 0);

        var btnMoi = CreateButton("Mới", Color.FromArgb(90, 103, 116));
        btnMoi.Name = "btnMoi";
        btnThem.Text = "Thêm";
        btnThem.Name = "btnThem";
        StyleButton(btnThem, Color.FromArgb(37, 132, 83));
        btnSua.Text = "Sửa";
        btnSua.Name = "btnSua";
        btnSua.Enabled = false;
        StyleButton(btnSua, Color.FromArgb(202, 132, 33));
        btnXoa.Text = "Xóa";
        btnXoa.Name = "btnXoa";
        btnXoa.Enabled = false;
        StyleButton(btnXoa, Color.FromArgb(190, 62, 62));
        var btnThoat = CreateButton("Thoát", Color.FromArgb(85, 97, 110));
        btnThoat.Name = "btnThoat";
        actions.Controls.AddRange([btnMoi, btnThem, btnSua, btnXoa, btnThoat]);
        btnMoi.Click += (_, _) => ResetForm();
        btnThem.Click += AddBook;
        btnSua.Click += UpdateBook;
        btnXoa.Click += DeleteBook;
        btnThoat.Click += (_, _) => Close();

        var listGroup = new GroupBox
        {
            Text = "Danh sách sách",
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            BackColor = Color.White
        };
        dgvSach.Dock = DockStyle.Fill;
        listGroup.Controls.Add(dgvSach);
        root.Controls.Add(listGroup, 0, 2);
    }

    private void ConfigureGrid()
    {
        dgvSach.Name = "dgvSach";
        dgvSach.AllowUserToAddRows = false;
        dgvSach.AllowUserToDeleteRows = false;
        dgvSach.AutoGenerateColumns = false;
        dgvSach.BackgroundColor = Color.White;
        dgvSach.BorderStyle = BorderStyle.None;
        dgvSach.ReadOnly = true;
        dgvSach.MultiSelect = false;
        dgvSach.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSach.RowHeadersVisible = false;
        dgvSach.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvSach.ColumnHeadersHeight = 38;
        dgvSach.RowTemplate.Height = 34;
        dgvSach.EnableHeadersVisualStyles = false;
        dgvSach.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(231, 238, 245);
        dgvSach.ColumnHeadersDefaultCellStyle.Font = new Font(Font, FontStyle.Bold);
        dgvSach.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(BookGridRow.MaSach),
            HeaderText = "Mã sách",
            FillWeight = 80
        });
        dgvSach.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(BookGridRow.TenSach),
            HeaderText = "Tên sách",
            FillWeight = 180
        });
        dgvSach.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(BookGridRow.NgayNhap),
            HeaderText = "Ngày nhập",
            FillWeight = 90,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
        });
        dgvSach.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(BookGridRow.SoLuong),
            HeaderText = "Số lượng",
            FillWeight = 75
        });
        dgvSach.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(BookGridRow.DonGia),
            HeaderText = "Đơn giá",
            FillWeight = 90,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" }
        });
        dgvSach.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(BookGridRow.TenLoai),
            HeaderText = "Thể loại",
            FillWeight = 105
        });
    }

    private void WireEvents()
    {
        Load += LoadData;
        Shown += (_, _) => txtMaSach.Focus();
        dgvSach.SelectionChanged += SelectBook;
        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                Close();
            }
        };
        FormClosing += (_, e) =>
        {
            var answer = MessageBox.Show(
                this,
                "Bạn có chắc muốn thoát chương trình?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            e.Cancel = answer != DialogResult.Yes;
        };
    }

    private void LoadData(object? sender, EventArgs e)
    {
        try
        {
            _bookService.InitializeDatabase();
            cboTheLoai.DataSource = _bookService.GetCategories();
            cboTheLoai.DisplayMember = nameof(TheLoai.TenLoai);
            cboTheLoai.ValueMember = nameof(TheLoai.MaLoai);
            ReloadBooks();
            ResetForm();
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private void ReloadBooks()
    {
        dgvSach.DataSource = _bookService.GetBooks()
            .Select(book => new BookGridRow(
                book.MaSach,
                book.TenSach,
                book.NgayNhap,
                book.SoLuong,
                book.DonGia,
                book.TheLoai.TenLoai,
                book.MaLoai))
            .ToList();
        dgvSach.ClearSelection();
    }

    private void AddBook(object? sender, EventArgs e)
    {
        try
        {
            _bookService.AddBook(ReadBook());
            ReloadBooks();
            ResetForm();
            MessageBox.Show(this, "Thêm sách thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private void UpdateBook(object? sender, EventArgs e)
    {
        try
        {
            _bookService.UpdateBook(ReadBook());
            ReloadBooks();
            ResetForm();
            MessageBox.Show(this, "Cập nhật sách thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private void DeleteBook(object? sender, EventArgs e)
    {
        if (dgvSach.CurrentRow?.DataBoundItem is not BookGridRow selected)
        {
            return;
        }

        var answer = MessageBox.Show(
            this,
            $"Bạn có chắc muốn xóa sách \"{selected.TenSach}\"?",
            "Xác nhận xóa",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);
        if (answer != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _bookService.DeleteBook(selected.MaSach);
            ReloadBooks();
            ResetForm();
            MessageBox.Show(this, "Xóa sách thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private void SelectBook(object? sender, EventArgs e)
    {
        if (dgvSach.CurrentRow?.DataBoundItem is not BookGridRow selected)
        {
            return;
        }

        txtMaSach.Text = selected.MaSach;
        txtMaSach.ReadOnly = true;
        txtTenSach.Text = selected.TenSach;
        dtpNgayNhap.Value = selected.NgayNhap;
        txtSoLuong.Text = selected.SoLuong.ToString(CultureInfo.CurrentCulture);
        txtDonGia.Text = selected.DonGia.ToString(CultureInfo.CurrentCulture);
        cboTheLoai.SelectedValue = selected.MaLoai;
        btnThem.Enabled = false;
        btnSua.Enabled = true;
        btnXoa.Enabled = true;
    }

    private Sach ReadBook()
    {
        if (!int.TryParse(txtSoLuong.Text.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out var quantity))
        {
            throw new ArgumentException("Số lượng phải là số nguyên lớn hơn 0.");
        }

        if (!decimal.TryParse(txtDonGia.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var price))
        {
            throw new ArgumentException("Đơn giá phải là số lớn hơn 0.");
        }

        if (cboTheLoai.SelectedValue is not string categoryId)
        {
            throw new ArgumentException("Vui lòng chọn thể loại.");
        }

        return new Sach
        {
            MaSach = txtMaSach.Text,
            TenSach = txtTenSach.Text,
            NgayNhap = dtpNgayNhap.Value.Date,
            SoLuong = quantity,
            DonGia = price,
            MaLoai = categoryId
        };
    }

    private void ResetForm()
    {
        dgvSach.ClearSelection();
        dgvSach.CurrentCell = null;
        cboTheLoai.SelectedIndex = -1;
        txtMaSach.Clear();
        txtMaSach.ReadOnly = false;
        txtTenSach.Clear();
        dtpNgayNhap.Value = DateTime.Today;
        txtSoLuong.Clear();
        txtDonGia.Clear();
        btnThem.Enabled = true;
        btnSua.Enabled = false;
        btnXoa.Enabled = false;
        txtMaSach.Focus();
    }

    private void ShowError(Exception exception)
    {
        MessageBox.Show(
            this,
            exception.GetBaseException().Message,
            "Không thể hoàn tất thao tác",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private static void ConfigureInput(Control input)
    {
        input.Dock = DockStyle.Fill;
        input.Margin = new Padding(4, 6, 12, 4);
        input.Font = new Font("Segoe UI", 10F);
    }

    private static void AddField(
        TableLayoutPanel layout,
        string title,
        Control input,
        int row,
        int labelColumn,
        int columnSpan = 1)
    {
        layout.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(4, 2, 4, 2)
        }, labelColumn, row);
        layout.Controls.Add(input, labelColumn + 1, row);
        if (columnSpan > 1)
        {
            layout.SetColumnSpan(input, columnSpan);
        }
    }

    private static Button CreateButton(string text, Color color)
    {
        var button = new Button { Text = text };
        StyleButton(button, color);
        return button;
    }

    private static void StyleButton(Button button, Color color)
    {
        button.Width = 125;
        button.Height = 38;
        button.Margin = new Padding(0, 0, 0, 8);
        button.BackColor = color;
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
    }

    private sealed record BookGridRow(
        string MaSach,
        string TenSach,
        DateTime NgayNhap,
        int SoLuong,
        decimal DonGia,
        string TenLoai,
        string MaLoai);
}
