namespace FieldOps.Domain.Enums;

public enum WorkOrderStatus { Draft, Scheduled, Dispatched, InProgress, Completed, Cancelled, OnHold }
public enum InvoiceStatus { Draft, Sent, PartiallyPaid, Paid, Overdue, Cancelled }
public enum PaymentMethod { Cash, BankTransfer, Card, Cheque, Online }
public enum UserRole { TenantAdmin, BranchManager, HRManager, Supervisor, Accountant }
public enum FollowUpType { PostServiceRating, OverdueInvoice, RebookingReminder, SlaBreachAlert }
public enum FollowUpChannel { WhatsApp, SMS, Email }
public enum ClientType { Individual, Corporate }
public enum WorkerStatus { Active, OnLeave, Terminated, Probation }
public enum AttendanceStatus { Present, Absent, HalfDay, OnLeave, PublicHoliday }
public enum DocumentType { Passport, EmiratesID, VisaPage, HealthCard, DrivingLicense, Contract }
public enum Emirates { Dubai, Sharjah, AbuDhabi, Ajman, RAK, Fujairah, UmmAlQuwain }
