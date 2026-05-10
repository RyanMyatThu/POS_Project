"use client";

import { useState, useEffect } from "react";
import { useAuth } from "@/lib/auth-context";
import { authApi, pointsApi } from "@/lib/api";
import { UserRole } from "@/lib/types";
import type { CustomerTier } from "@/lib/types";
import { Card, CardHeader } from "@/components/ui/Card";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { Select } from "@/components/ui/Select";
import { AnimatedPage } from "@/components/ui/AnimatedPage";
import { toast } from "@/components/ui/Toast";
import { motion, AnimatePresence } from "framer-motion";
import { UserPlus, Phone, Lock, User, Mail, Shield, Trash2, RefreshCw } from "lucide-react";
import type { UserDTO } from "@/lib/types";

export default function UsersPage() {
  const { isAdmin, isStaff } = useAuth();

  // Account type selector
  const [accountType, setAccountType] = useState<"Staff" | "Customer">(
    isAdmin ? "Staff" : "Customer"
  );

  // Common form fields
  const [name, setName] = useState("");
  const [mobileNum, setMobileNum] = useState("");
  const [password, setPassword] = useState("");

  // Customer-only fields
  const [email, setEmail] = useState("");
  const [tier, setTier] = useState<CustomerTier>("None");

  const [isLoading, setIsLoading] = useState(false);
  const [isFetching, setIsFetching] = useState(false);
  const [users, setUsers] = useState<UserDTO[]>([]);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const fetchUsers = async () => {
    if (!isAdmin) return;
    setIsFetching(true);
    try {
      const res = await authApi.getUsers();
      if (res.isSuccess && res.data) {
        setUsers(res.data);
      }
    } catch {
      toast("error", "Failed to fetch users");
    } finally {
      setIsFetching(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, [isAdmin]);

  const accountOptions = isAdmin
    ? [
        { value: "Staff", label: "Staff Account" },
        { value: "Customer", label: "Customer Account" },
      ]
    : [{ value: "Customer", label: "Customer Account" }];

  const tierOptions = [
    { value: "None", label: "None" },
    { value: "Silver", label: "Silver" },
    { value: "Gold", label: "Gold" },
    { value: "Platinum", label: "Platinum" },
  ];

  const validate = () => {
    const newErrors: Record<string, string> = {};
    if (!name.trim()) newErrors.name = "Name is required";
    if (!mobileNum.trim()) newErrors.mobileNum = "Mobile number is required";
    if (!password) newErrors.password = "Password is required";
    else if (password.length < 6)
      newErrors.password = "Password must be at least 6 characters";
    if (accountType === "Customer" && !email.trim())
      newErrors.email = "Email is required for customer accounts";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const clearError = (field: string) => {
    if (errors[field]) {
      setErrors((prev) => {
        const copy = { ...prev };
        delete copy[field];
        return copy;
      });
    }
  };

  const resetForm = () => {
    setName("");
    setMobileNum("");
    setPassword("");
    setEmail("");
    setTier("None");
    setErrors({});
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    setIsLoading(true);
    try {
      if (accountType === "Staff") {
        // Staff: single endpoint
        const res = await authApi.register({
          name,
          mobileNum,
          password,
          role: UserRole.Staff,
        });
        if (res.isSuccess) {
          toast("success", "Staff account created successfully!");
          resetForm();
          fetchUsers();
        } else {
          toast("error", res.message || "Failed to create staff account");
        }
      } else {
        // Customer: two-step
        // Step 1: Create auth account
        const authRes = await authApi.register({
          name,
          mobileNum,
          password,
          role: UserRole.Customer,
        });

        if (!authRes.isSuccess) {
          toast("error", authRes.message || "Failed to create customer account");
          return;
        }

        // Step 2: Create loyalty/points account
        try {
          const pointsRes = await pointsApi.createAccount({
            tier,
            mobile: mobileNum,
            email,
          });
          if (pointsRes.isSuccess) {
            toast("success", "Customer account and loyalty profile created!");
          } else {
            toast(
              "warning",
              "Account created but loyalty setup failed: " + pointsRes.message
            );
          }
        } catch {
          toast(
            "warning",
            "Customer auth account created, but loyalty setup failed. You can retry from Loyalty page."
          );
        }

        fetchUsers();
        resetForm();
      }
    } catch (err: unknown) {
      const message =
        (err as { response?: { data?: { message?: string } } })?.response?.data
          ?.message || "Account creation failed";
      toast("error", message);
    } finally {
      setIsLoading(false);
    }
  };

  if (!isAdmin && !isStaff) {
    return (
      <AnimatedPage>
        <div className="flex flex-col items-center justify-center h-[60vh] gap-4">
          <Shield size={48} className="text-[var(--text-tertiary)]" />
          <h2 className="text-xl font-semibold text-[var(--text-primary)]">
            Access Restricted
          </h2>
          <p className="text-[var(--text-secondary)]">
            User management is available for Admin and Staff only.
          </p>
        </div>
      </AnimatedPage>
    );
  }

  return (
    <AnimatedPage>
      <div className="space-y-6 max-w-2xl mx-auto">
        {/* Header */}
        <div>
          <h2 className="text-2xl font-bold text-[var(--text-primary)]">
            Create User Account
          </h2>
          <p className="text-sm text-[var(--text-secondary)] mt-1">
            {isAdmin
              ? "Create staff or customer accounts"
              : "Create customer accounts"}
          </p>
        </div>

        {/* Account Type Selector */}
        <Card padding="lg">
          <CardHeader
            title="Account Type"
            subtitle="Select the type of account to create"
            action={<UserPlus size={18} className="text-[var(--text-tertiary)]" />}
          />

          <Select
            id="account-type"
            value={accountType}
            onChange={(e) => {
              setAccountType(e.target.value as "Staff" | "Customer");
              setErrors({});
            }}
            options={accountOptions}
          />
        </Card>

        {/* Create Form */}
        <Card padding="lg">
          <CardHeader
            title={`New ${accountType} Account`}
            subtitle={
              accountType === "Staff"
                ? "Create login credentials for a new staff member"
                : "Create login + loyalty profile for a new customer"
            }
          />

          <form onSubmit={handleSubmit} className="space-y-4">
            <Input
              label="Full Name"
              placeholder="Enter full name"
              value={name}
              onChange={(e) => {
                setName(e.target.value);
                clearError("name");
              }}
              error={errors.name}
              icon={<User size={18} />}
              id="user-name"
            />

            <Input
              label="Mobile Number"
              type="tel"
              placeholder="09xxxxxxxxx"
              value={mobileNum}
              onChange={(e) => {
                setMobileNum(e.target.value);
                clearError("mobileNum");
              }}
              error={errors.mobileNum}
              icon={<Phone size={18} />}
              id="user-mobile"
            />

            <Input
              label="Password"
              type="password"
              placeholder="Min. 6 characters"
              value={password}
              onChange={(e) => {
                setPassword(e.target.value);
                clearError("password");
              }}
              error={errors.password}
              icon={<Lock size={18} />}
              id="user-password"
            />

            {/* Customer-only fields */}
            <AnimatePresence>
              {accountType === "Customer" && (
                <motion.div
                  initial={{ opacity: 0, height: 0 }}
                  animate={{ opacity: 1, height: "auto" }}
                  exit={{ opacity: 0, height: 0 }}
                  transition={{ duration: 0.3 }}
                  className="space-y-4 overflow-hidden"
                >
                  <div className="pt-2 border-t border-[var(--border-primary)]">
                    <p className="text-xs text-[var(--text-tertiary)] mb-4">
                      Loyalty profile details
                    </p>
                  </div>

                  <Input
                    label="Email Address"
                    type="email"
                    placeholder="customer@example.com"
                    value={email}
                    onChange={(e) => {
                      setEmail(e.target.value);
                      clearError("email");
                    }}
                    error={errors.email}
                    icon={<Mail size={18} />}
                    id="user-email"
                  />

                  <Select
                    label="Loyalty Tier"
                    id="user-tier"
                    value={tier}
                    onChange={(e) => setTier(e.target.value as CustomerTier)}
                    options={tierOptions}
                  />
                </motion.div>
              )}
            </AnimatePresence>

            <div className="pt-4">
              <Button
                type="submit"
                size="lg"
                isLoading={isLoading}
                className="w-full"
                icon={!isLoading ? <UserPlus size={18} /> : undefined}
              >
                Create {accountType} Account
              </Button>
            </div>
          </form>
        </Card>

        {/* User Management List - Admin Only */}
        {isAdmin && (
          <Card padding="lg">
            <CardHeader
              title="Manage Users"
              subtitle="View and manage existing system accounts"
              action={
                <button 
                  onClick={fetchUsers} 
                  disabled={isFetching}
                  className="p-2 rounded-lg hover:bg-[var(--bg-hover)] text-[var(--text-tertiary)] transition-colors"
                >
                  <RefreshCw size={18} className={isFetching ? "animate-spin" : ""} />
                </button>
              }
            />

            <div className="overflow-x-auto -mx-6">
              <table className="w-full text-left border-collapse">
                <thead>
                  <tr className="border-b border-[var(--border-primary)]">
                    <th className="px-6 py-3 text-xs font-bold text-[var(--text-tertiary)] uppercase tracking-widest">User</th>
                    <th className="px-6 py-3 text-xs font-bold text-[var(--text-tertiary)] uppercase tracking-widest">Role</th>
                    <th className="px-6 py-3 text-xs font-bold text-[var(--text-tertiary)] uppercase tracking-widest">Mobile</th>
                    <th className="px-6 py-3 text-xs font-bold text-[var(--text-tertiary)] uppercase tracking-widest">Created</th>
                    <th className="px-6 py-3 text-xs font-bold text-[var(--text-tertiary)] uppercase tracking-widest text-right">Actions</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-[var(--border-primary)]">
                  {users.length === 0 ? (
                    <tr>
                      <td colSpan={5} className="px-6 py-8 text-center text-[var(--text-tertiary)]">
                        {isFetching ? "Loading users..." : "No users found"}
                      </td>
                    </tr>
                  ) : (
                    users.map((u) => (
                      <tr key={u.id} className="hover:bg-[var(--bg-hover)] transition-colors">
                        <td className="px-6 py-4">
                          <div className="font-medium text-[var(--text-primary)]">{u.name}</div>
                        </td>
                        <td className="px-6 py-4">
                          <span className={`
                            px-2 py-0.5 rounded-full text-[10px] font-bold uppercase
                            ${u.role === UserRole.Admin ? "bg-red-500/10 text-red-500" : 
                              u.role === UserRole.Staff ? "bg-blue-500/10 text-blue-500" : 
                              "bg-green-500/10 text-green-500"}
                          `}>
                            {UserRole[u.role]}
                          </span>
                        </td>
                        <td className="px-6 py-4 text-sm text-[var(--text-secondary)]">
                          {u.mobileNum}
                        </td>
                        <td className="px-6 py-4 text-sm text-[var(--text-tertiary)]">
                          {new Date(u.createdAt).toLocaleDateString()}
                        </td>
                        <td className="px-6 py-4 text-right">
                          <button
                            onClick={async () => {
                              if (confirm(`Are you sure you want to delete ${u.name}?`)) {
                                try {
                                  const res = await authApi.deleteUser(u.id);
                                  if (res.isSuccess) {
                                    toast("success", "User deleted successfully");
                                    fetchUsers();
                                  } else {
                                    toast("error", res.message);
                                  }
                                } catch {
                                  toast("error", "Failed to delete user");
                                }
                              }
                            }}
                            className="p-2 text-[var(--accent-danger)] hover:bg-[var(--accent-danger)]/10 rounded-lg transition-colors"
                          >
                            <Trash2 size={18} />
                          </button>
                        </td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </Card>
        )}
      </div>
    </AnimatedPage>
  );
}
